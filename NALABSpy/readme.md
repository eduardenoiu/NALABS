# NALABS: Natural Language Analysis of Bad Smells in Software Requirements

NALABS is a Python script designed to analyze software requirements and identify potential bad smells that may indicate issues in the requirement's quality or clarity. The script reads requirements from an Excel file, processes them using various linguistic techniques, and outputs an Excel file containing the detected bad smells for each requirement.

## Features

NALABS performs the following analysis on each requirement:

1. Detects ambiguous words
2. Measures readability using the Flesch Reading Ease score
3. Calculates subjectivity to identify potential weaknesses in the requirement
4. Ensures that the requirement contains specific keywords
5. Checks if the requirement is security-related

## Dependencies

- pandas
- openpyxl
- spacy
- textstat
- textblob

Install the dependencies using the following command:

pip install pandas openpyxl spacy textstat textblob


## Usage

1. Open the `nalabs.py` file in a text editor or an IDE.
2. Update the `input_file`, `id_column`, `text_column`, and `output_file` variables with your specific Excel file names and columns.
3. Save the changes.
4. Run the script using the following command:

```bash
python NALABS.py <input_file> <id_column> <text_column> <output_file>

Details:
input_file = 'requirements.xlsx'
id_column = 'ID'
text_column = 'Requirement'
output_file = 'bad_smells.xlsx'

run_nalabs(input_file, id_column, text_column, output_file)
