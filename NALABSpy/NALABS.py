#! python
import argparse
import sys
from typing import List, Tuple

from data_file_handlers import (
    read_requirements_from_excel,
    write_bad_smells_to_excel,
    write_bad_smells_to_json,
    read_requirements_from_json,
)
from nalabs_rules import (
    apply_smell_rules,
    make_smell_entry,
    BAD_SMELL_DEFAULT_FIELDS, apply_all_rules,
)

DEFAULT_TEXT_COLUMN_NAME = "Requirement"
DEFAULT_ID_COLUMN_NAME = "ID"
DEFAULT_OUTPUT_FILE_PATH = "bad_smells.xlsx"
DEFAULT_INPUT_FILE_PATH = "requirements.xlsx"

nalabs_description = """
NALABS: Natural Language Analysis of Bad Smells in Software Requirements\n
This function is designed to analyze software requirements and identify potential bad smells\n
that may indicate issues in the requirement's quality or clarity. The script reads requirements\n
from a file, processes them using various linguistic techniques, and outputs a\n
file containing the detected bad smells for each requirement.\n
\n
The script performs the following analysis on each requirement:\n
* Detects ambiguous words\n
* Measures readability using the Flesch Reading Ease score\n
* Calculates subjectivity to identify potential weaknesses in the requirement\n
* Ensures that the requirement contains specific keywords\n
* Checks if the requirement is security-related\n
"""

arg_parser = argparse.ArgumentParser(description=nalabs_description)
arg_parser.add_argument(
    "-i", "--input-file", required=False, default=DEFAULT_INPUT_FILE_PATH
)
arg_parser.add_argument(
    "-o", "--output-file", required=False, default=DEFAULT_OUTPUT_FILE_PATH
)
arg_parser.add_argument("--id-header", required=False, default=DEFAULT_ID_COLUMN_NAME)
arg_parser.add_argument(
    "--text-header", required=False, default=DEFAULT_TEXT_COLUMN_NAME
)
arg_parser.add_argument("-v", "--verbose", required=False, action="store_true")
arg_parser.add_argument("-A", "--all-checks", required=False, action="store_true")


def main():
    def detect_bad_smells(smell_detection_function, requirements: List[Tuple[str, str]], hide_non_issues=True):
        bad_smells = []

        for req_id, requirement in requirements:
            if not isinstance(requirement, str):
                if args.verbose:
                    print(
                        "Skipping requirement that does not appear to be a string: "
                        + requirement
                    )
                continue
            bad_smell_entry = make_smell_entry(req_id, requirement)
            bad_smell_entry = smell_detection_function(bad_smell_entry)

            if (
                hide_non_issues
                and len(bad_smell_entry) <= len(BAD_SMELL_DEFAULT_FIELDS)
            ):
                pass
            else:
                bad_smells.append(bad_smell_entry)

        return bad_smells

    def run_nalabs(input_file, id_column, text_column, output_file, verbose_mode=False):
        smell_detector = apply_all_rules if args.all_checks else apply_smell_rules

        if verbose_mode:
            print("Reading requirements from file: " + input_file)
        requirements = read_requirements_from_json(input_file, id_column, text_column)
        if verbose_mode:
            print("Running smell checker")
        bad_smells = detect_bad_smells(smell_detector, requirements)
        if verbose_mode:
            print("Printing report to output file: " + output_file)
        write_bad_smells_to_json(bad_smells, output_file)
        if verbose_mode:
            print("Work done. Exiting!")
        return

    args = arg_parser.parse_args()
    input_file = args.input_file
    output_file = args.output_file
    text_column = args.text_header
    id_column = args.id_header

    run_nalabs(input_file, id_column, text_column, output_file, args.verbose)


if "__main__" == __name__:
    main()
