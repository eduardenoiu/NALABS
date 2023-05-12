



import pandas as pd
import re
import spacy
from spacy.tokens import Doc
from textstat import flesch_reading_ease
from textblob import TextBlob
import sys

def main():
    """
    NALABS: Natural Language Analysis of Bad Smells in Software Requirements

    This script is designed to analyze software requirements and identify potential bad smells
    that may indicate issues in the requirement's quality or clarity. The script reads requirements
    from an Excel file, processes them using various linguistic techniques, and outputs an Excel
    file containing the detected bad smells for each requirement.

    The script performs the following analysis on each requirement:
    3. Detects ambiguous words
    4. Measures readability using the Flesch Reading Ease score
    5. Calculates subjectivity to identify potential weaknesses in the requirement
    7. Ensures that the requirement contains specific keywords
    8. Checks if the requirement is security-related

    To use this script, simply update the input_file, id_column, text_column, and output_file variables
    with your specific Excel file names and columns, and then run the script.

    Dependencies:
    - pandas
    - openpyxl
    - spacy
    - textstat
    - textblob

    Example usage:

    input_file = 'requirements.xlsx'
    id_column = 'ID'
    text_column = 'Requirement'
    output_file = 'bad_smells.xlsx'

    run_nalabs(input_file, id_column, text_column, output_file)

    """
    # Load spaCy's English language model
    nlp = spacy.load('en_core_web_sm')

    # Register 'subjectivity' extension attribute
    Doc.set_extension('subjectivity', default=0.0)

    def read_requirements_from_excel(file_path, id_column, text_column):
        df = pd.read_excel(file_path)
        requirements = []
        for index, row in df.iterrows():
            req_id = row[id_column]
            requirement = row[text_column]
            requirements.append((req_id, requirement))
        return requirements

    def detect_bad_smells(requirements):
        bad_smells = []
        ambiguous_words = ['may', 'could','has to', 'have to', 'might', 'will', 'should have','must have', 'all the other', 'all other',
                        'based on', 'some', 'appropriate', 'as a', 'as an', 'a minimum', 'up to','adequate', 'as applicable', 'be able to',
                        'be capable', 'but not limited to', 'capability of', 'capability to', 'effective', 'normal']  # Add your list of ambiguous words here
        requirement_keywords = ['shall', 'must', 'should', 'will', 'requires', 'necessitates', 'needs to', 'is required to'] # Add your list of requirement words here
        security_keywords = ['security', 'secure', 'confidentiality', 'integrity', 'availability', 'authentication', 'authorization',
                            'encryption', 'access control', 'audit', 'firewall', 'intrusion detection', 'vulnerability', 'patching',
                            'secure communication', 'privacy', 'compliance', 'risk assessment', 'incident response', 'disaster recovery',
                            'secure coding'] # Add your list of security words here

        for req_id, requirement in requirements:
            if isinstance(requirement, str):
                bad_smell_entry = {
                    'ID': req_id,
                    'Requirement': requirement
                }


                # Rule 1: Check for ambiguous words using spaCy for part-of-speech tagging
                doc = nlp(requirement)
                ambiguous_word_matches = [token.text for token in doc if token.text.lower() in ambiguous_words]
                if ambiguous_word_matches:
                    bad_smell_entry['Ambiguity Detected'] = ', '.join(ambiguous_word_matches)

                # Rule 2: Check for low readability (Flesch Reading Ease score)
                reading_ease_score = flesch_reading_ease(requirement)
                if reading_ease_score < 30:  # Adjust the threshold as needed
                    bad_smell_entry['Low Readability'] = f'Flesch Reading Ease: {reading_ease_score:.2f}'

                # Rule 3: Check for subjectivity 
                blob = TextBlob(requirement)
                subjectivity_score = blob.sentiment.subjectivity
                if subjectivity_score > 0.5:  # Adjust the threshold as needed
                    bad_smell_entry['Subjectivity Detected'] = f'Subjectivity Score: {subjectivity_score:.2f}'


                # Rule 4: Check if requirement text contains requirement keywords
                has_requirement_keywords = any(keyword in requirement.lower() for keyword in requirement_keywords)
                if not has_requirement_keywords:
                    bad_smell_entry['Not a Requirement'] = True

                # Rule 5: Check if requirement is security-related
                is_security_related = any(keyword in requirement.lower() for keyword in security_keywords)
                if is_security_related:
                    bad_smell_entry['Security Related'] = True

                if len(bad_smell_entry) > 2:
                    bad_smells.append(bad_smell_entry)


        return bad_smells

    def write_bad_smells_to_excel(bad_smells, output_file):
        if len(bad_smells) == 0:
            return

        df = pd.DataFrame(bad_smells)
        columns = ['ID', 'Requirement', 'Ambiguity Detected',
                'Low Readability', 'Subjectivity Detected',
                'Not a Requirement', 'Security Related']

        # Reorder the DataFrame columns
        df = df.reindex(columns=columns)
        df.to_excel(output_file, index=False)

        

    def run_nalabs(input_file, id_column, text_column, output_file):
        requirements = read_requirements_from_excel(input_file, id_column, text_column)
        bad_smells = detect_bad_smells(requirements)
        write_bad_smells_to_excel(bad_smells, output_file)

    if len(sys.argv) > 4:
        #__file__ <input_file> <id_column> <text_column> <output_file>
        input_file = sys.argv[1]
        id_column = sys.argv[2]
        text_column = sys.argv[3]
        output_file = sys.argv[4]
    
    # Example usage
    else:
        input_file = 'requirements.xlsx'
        id_column = 'ID'
        text_column = 'Requirement'
        output_file = 'bad_smells.xlsx'
    

    run_nalabs(input_file, id_column, text_column, output_file)

if '__main__' == __name__:
    main()