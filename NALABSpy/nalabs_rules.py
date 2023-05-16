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


def check_ambiguity_rule(requirement):
    # Rule 1: Check for ambiguous words using spaCy for part-of-speech tagging
    doc = nlp(requirement)
    ambiguous_word_matches = [
        token.text for token in doc if token.text.lower() in ambiguous_words
    ]
    return ", ".join(ambiguous_word_matches) if ambiguous_word_matches else ""


def check_reading_score_rule(requirement):
    # Rule 2: Check for low readability (Flesch Reading Ease score)
    reading_ease_score = flesch_reading_ease(requirement)
    return f"Flesch Reading Ease: {reading_ease_score:.2f}" if (
        # Adjust the threshold as needed
        reading_ease_score < DEFAULT_MINIMUM_REQUIRED_READING_SCORE
    ) else ""



def check_subjectivity_rule(requirement):
    # Rule 3: Check for subjectivity
    blob = TextBlob(requirement)
    subjectivity_score = blob.sentiment.subjectivity
    return f"Subjectivity Score: {subjectivity_score:.2f}" if (
        # Adjust the threshold as needed
        subjectivity_score > DEFAULT_MAXIMUM_ALLOWED_SUBJECTIVITY_SCORE
    ) else ""


def check_is_requirement_rule(requirement):
    # Rule 4: Check if requirement text contains requirement keywords
    return not contains_any_keywords(requirement, requirement_keywords)


def check_security_related_rule(requirement):
    # Rule 5: Check if requirement is security-related
    return contains_any_keywords(requirement, security_keywords)



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
        violates_rule_message = rule_check(smell_entry["Requirement"])
        if violates_rule_message:
            smell_entry[dh] = violates_rule_message
    return smell_entry
