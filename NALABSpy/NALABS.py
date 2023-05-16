import sys
from typing import List, Tuple

from data_file_handlers import read_requirements_from_excel, write_bad_smells_to_excel, write_bad_smells_to_json, \
    read_requirements_from_json
from nalabs_rules import (
    apply_all_rules,
    make_smell_entry,
    BAD_SMELL_DEFAULT_FIELD_AMOUNT,
)

DEFAULT_OUTPUT_FILE_PATH = "bad_smells.xlsx"
DEFAULT_INPUT_FILE_PATH = "requirements.xlsx"

def main():
    def detect_bad_smells(requirements: List[Tuple[str, str]], hide_non_issues=True):
        bad_smells = []

        for req_id, requirement in requirements:
            if not isinstance(requirement, str):
                if verbose_mode:
                    print(
                        "Skipping requirement that does not appear to be a string: "
                        + requirement
                    )
                continue
            bad_smell_entry = make_smell_entry(req_id, requirement)
            bad_smell_entry = apply_all_rules(bad_smell_entry)

            if hide_non_issues and len(bad_smell_entry) <= BAD_SMELL_DEFAULT_FIELD_AMOUNT:
                pass
            else:
                bad_smells.append(bad_smell_entry)

        return bad_smells

    def run_nalabs(input_file, id_column, text_column, output_file, verbose_mode=False):
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

        Example usage:

        input_file = 'requirements.xlsx'
        id_column = 'ID'
        text_column = 'Requirement'
        output_file = 'bad_smells.xlsx'

        run_nalabs(input_file, id_column, text_column, output_file)

        """
        if verbose_mode:
            print("Reading requirements from file: " + input_file)
        requirements = read_requirements_from_json(input_file, id_column, text_column)
        if verbose_mode:
            print("Running smell checker")
        bad_smells = detect_bad_smells(requirements)
        if verbose_mode:
            print("Printing report to output file: " + output_file)
        write_bad_smells_to_json(bad_smells, output_file)
        if verbose_mode:
            print("Work done. Exiting!")
        return

    if len(sys.argv) > 4:
        # __file__ <input_file> <id_column> <text_column> <output_file>
        input_file = sys.argv[1]
        id_column = sys.argv[2]
        text_column = sys.argv[3]
        output_file = sys.argv[4]

    # Example usage
    else:
        input_file = DEFAULT_INPUT_FILE_PATH
        id_column = "ID"
        text_column = "Requirement"
        output_file = DEFAULT_OUTPUT_FILE_PATH

    verbose_mode = True if "--verbose" in sys.argv else False

    run_nalabs(input_file, id_column, text_column, output_file, verbose_mode)


if "__main__" == __name__:
    main()
