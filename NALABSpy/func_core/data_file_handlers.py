import json

import pandas as pd
from .nalabs_rules import SMELL_DATA_HEADERS


def read_requirements_from_excel(file_path, id_column: str, text_column: str):
    df = pd.read_excel(file_path)
    requirements = []
    for index, row in df.iterrows():
        req_id = row[id_column]
        requirement = row[text_column]
        requirements.append((req_id, requirement))
    return requirements


def read_requirements_from_json(file_path, id_column, text_column):
    with open(file_path, "r") as fp:
        df = json.load(fp)
    requirements = []
    for row in df:
        req_id = row[id_column]
        requirement = row[text_column]
        requirements.append((req_id, requirement))
    return requirements


def write_bad_smells_to_excel(bad_smells, output_file):
    if len(bad_smells) == 0:
        return

    df = pd.DataFrame(bad_smells)

    # Reorder the DataFrame columns
    df = df.reindex(columns=SMELL_DATA_HEADERS)
    df.to_excel(output_file, index=False)


def write_bad_smells_to_json(bad_smells, output_file):
    if len(bad_smells) == 0:
        return
    with open(output_file, "w") as fp:
        json.dump(obj=bad_smells, fp=fp, indent=2)
