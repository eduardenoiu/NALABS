import spacy
from spacy.tokens import Doc
from textstat import flesch_reading_ease
from textblob import TextBlob

# Load spaCy's English language model
nlp = spacy.load("en_core_web_sm")

# Register 'subjectivity' extension attribute
Doc.set_extension("subjectivity", default=0.0)

ambiguous_words = {
    "may",
    "could",
    "has to",
    "have to",
    "might",
    "will",
    "should have",
    "must have",
    "all the other",
    "all other",
    "based on",
    "some",
    "appropriate",
    "as a",
    "as an",
    "a minimum",
    "up to",
    "adequate",
    "as applicable",
    "be able to",
    "be capable",
    "but not limited to",
    "capability of",
    "capability to",
    "effective",
    "normal",
}  # Add your list of ambiguous words here
requirement_keywords = {
    "shall",
    "must",
    "should",
    "will",
    "requires",
    "necessitates",
    "needs to",
    "is required to",
}  # Add your list of requirement words here
security_keywords = {
    "security",
    "secure",
    "confidentiality",
    "integrity",
    "availability",
    "authentication",
    "authorization",
    "encryption",
    "access control",
    "audit",
    "firewall",
    "intrusion detection",
    "vulnerability",
    "patching",
    "secure communication",
    "privacy",
    "compliance",
    "risk assessment",
    "incident response",
    "disaster recovery",
    "secure coding",
}  # Add your list of security words here


DEFAULT_MINIMUM_REQUIRED_READING_SCORE = 30
DEFAULT_MAXIMUM_ALLOWED_SUBJECTIVITY_SCORE = 0.5


def make_smell_entry(id: str, smell_content: str):
    return {"ID": id, "Requirement": smell_content}

BAD_SMELL_DEFAULT_FIELD_AMOUNT = len(make_smell_entry("dummy", "dummy"))


def contains_any_keywords(requirement, keywords):
    return any(keyword in requirement.lower() for keyword in keywords)


def check_ambiguity_rule(_bad_smell_entry, data_header="Ambiguity Detected"):
    # Rule 1: Check for ambiguous words using spaCy for part-of-speech tagging
    doc = nlp(_bad_smell_entry["Requirement"])
    ambiguous_word_matches = [
        token.text for token in doc if token.text.lower() in ambiguous_words
    ]
    if ambiguous_word_matches:
        _bad_smell_entry[data_header] = ", ".join(ambiguous_word_matches)
    return _bad_smell_entry


def check_reading_score_rule(_bad_smell_entry, data_header="Low Readability"):
    # Rule 2: Check for low readability (Flesch Reading Ease score)
    reading_ease_score = flesch_reading_ease(_bad_smell_entry["Requirement"])
    if (
        reading_ease_score < DEFAULT_MINIMUM_REQUIRED_READING_SCORE
    ):  # Adjust the threshold as needed
        _bad_smell_entry[
            data_header
        ] = f"Flesch Reading Ease: {reading_ease_score:.2f}"
    return _bad_smell_entry


def check_subjectivity_rule(_bad_smell_entry, data_header="Subjectivity Detected"):
    # Rule 3: Check for subjectivity
    blob = TextBlob(_bad_smell_entry["Requirement"])
    subjectivity_score = blob.sentiment.subjectivity
    if (
        subjectivity_score > DEFAULT_MAXIMUM_ALLOWED_SUBJECTIVITY_SCORE
    ):  # Adjust the threshold as needed
        _bad_smell_entry[
            data_header
        ] = f"Subjectivity Score: {subjectivity_score:.2f}"
    return _bad_smell_entry


def check_is_requirement_rule(_bad_smell_entry, data_header="Not a Requirement"):
    # Rule 4: Check if requirement text contains requirement keywords
    if not contains_any_keywords(_bad_smell_entry["Requirement"], requirement_keywords):
        _bad_smell_entry[data_header] = True
    return _bad_smell_entry


def check_security_related_rule(_bad_smell_entry, data_header="Security Related"):
    requirement = _bad_smell_entry["Requirement"]
    # Rule 5: Check if requirement is security-related
    if contains_any_keywords(requirement, security_keywords):
        _bad_smell_entry[data_header] = True
    return _bad_smell_entry



SMELL_DATA_HEADERS = [
    "ID",
    "Requirement",
    "Ambiguity Detected",
    "Low Readability",
    "Subjectivity Detected",
    "Security Related",
    "Not a Requirement",
]
all_rules_functions = [
        check_ambiguity_rule,
        check_reading_score_rule,
        check_subjectivity_rule,
        check_security_related_rule,
        check_is_requirement_rule,
]
def apply_all_rules(smell_entry):
    # Each rule shall mutate the bad smell data entry
    for dh, rule_check in zip(SMELL_DATA_HEADERS[2:], all_rules_functions):
        smell_entry = rule_check(smell_entry, dh)
    return smell_entry
