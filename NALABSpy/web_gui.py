import os.path

from annotated_text import annotated_text
from streamlit import slider, text, file_uploader, dataframe, tabs, button
from func_core.nalabs_rules import apply_smell_rules, DEFAULT_MAXIMUM_ALLOWED_SUBJECTIVITY_SCORE
from func_core.data_file_handlers import read_requirements_from_json, read_requirements_from_excel
from NALABS import detect_bad_smells
from os import mkdir
import pandas as pd
def make_local_data_folder():
    """Creates a common local datastorage folder"""
    data_folder_name = "NALABS_RUNTIME_DATA"
    data_folder_path = os.path.join(os.getcwd(), data_folder_name)
    if os.path.exists(data_folder_path):
        return data_folder_path
    mkdir(data_folder_path)
    return data_folder_path
def info():
    """Print information text about the page"""
    annotated_text(
        "Hello ",
        ("<name>", "User"),
        "! ",
        "This is ",
        ("NALABS", "Smell Detection Tool"), " talking."
    )
def config():
    """Creates configuration header. Returns the set configuration values"""
    subjectivity_limit = slider(label="Maximum Allowed Subjectivity Score",
                                min_value=0.0,
                                max_value=1.0,
                                value=DEFAULT_MAXIMUM_ALLOWED_SUBJECTIVITY_SCORE,
                                step=0.01)

    def subjectivity_classifier(subj_val):
        if subj_val < 0.01:
            return "no"
        if subj_val < 0.15:
            return "a tiny amount of"
        if subj_val < 0.35:
            return "a small amount of"
        if subj_val < 0.50:
            return "some amount of"
        if subj_val < 0.70:
            return "a large amount of"
        return "a huge amount of"

    text(f"I will allow {subjectivity_classifier(subjectivity_limit)} subjectivity.")

    return {"Max_Subjectivity_Score": subjectivity_limit}
def copy_and_run_smells_on_files(data_folder, files, file_tabs):
    """Callback to populate the provided file tabs with data"""
    for i in range(len(files)):
        curr_tab = file_tabs[i]
        curr_file = files[i]
        curr_tab.text(f"You want me to process {curr_file.name}\nwhich is of type {curr_file.type}")
        with open(os.path.join(data_folder, curr_file.name), "wb") as fp:
            fp.write(curr_file.getvalue())
        requirements = read_requirements_from_json(os.path.join(data_folder, curr_file.name), "ID",
                                                   "Requirement") if "json" in curr_file.type else read_requirements_from_excel(
            os.path.join(data_folder, curr_file.name), "ID", "Requirement")
        smell_records = detect_bad_smells(smell_detection_function=apply_smell_rules, requirements=requirements)
        curr_tab.dataframe(pd.DataFrame(smell_records).reindex())
def render():
    """Main render function for page"""
    info()
    conf_options = config()
    data_folder = make_local_data_folder()
    files = file_uploader(label="Please provide files with requirements to scan:", type=["json", "xlsx"], accept_multiple_files=True)
    if len(files) == 0:
        return
    file_tabs = tabs([f.name for f in files])
    accept_button = button(label="Run Smells", on_click=copy_and_run_smells_on_files,args=(data_folder, files, file_tabs))


if __name__ == "__main__":
    render()

