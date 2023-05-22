import spacy
from spacy.tokens import Doc
from textstat import flesch_reading_ease
from textblob import TextBlob
from typing import Set, List
from functools import partial

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

optionality_keywords = {"can", "may", "optionally"}
conjunction_keywords = {"and", "after", "although", "as long as", "before", "but", "else", "if", "in order", "in case",
                        "nor", "or", "otherwise", "once", "since", "then", "though", "till", "unless", "until", "when",
                        "whenever", "where", "whereas", "wherever", "while", "yet"}
continuances_keywords = {"below", "as follows", "following", "listed", "in particular", "support", "and", ":"}
imperatives_keywords = {"must", "should", "could", "would", "can", "will", "may", "shall", "must", "is required to",
                        "are applicable", "are to", "responsible for", "will", "should"}
vagueness_keywords = {"may", "could", "has to", "have to", "might", "will", "should have", "must have", "all the other",
                      "all other", "based on", "some", "appropriate", "as a", "as an", "a minimum", "up to", "adequate",
                      "as applicable", "be able to", "be capable", "but not limited to", "capability of",
                      "capability to", "effective", "normal"}
references_keywords = {"e.g.", "i.e.", "for example", "for instance", "figure", "table", "note"}
subjectivity_keywords = {"similar", "better", "similarly", "worse", "having in mind", "take into account",
                         "take into consideration", "as possible"}
weakness_keywords = {"adequate", "as appropriate", "be able to", "be capable of", "capability", "effective",
                     "as required", "normal", "provide for", "timely", "easy to"}


def make_smell_entry(id: str, smell_content: str):
    return {"ID": id, "Requirement": smell_content}


BAD_SMELL_DEFAULT_FIELDS = make_smell_entry("dummy", "dummy").keys()


def contains_any_keywords(requirement, keywords):
    return any(keyword in requirement.lower() for keyword in keywords)


def number_of_forbidden_words(forbidden_phrases: Set[str], requirement: str):
    found_phrases = [(phrase, requirement.count(phrase)) for phrase in forbidden_phrases]
    return [(p, c) for p, c in found_phrases if c > 0]


def count_forbidden_words(forbidden_phrases, requirement):
    res = number_of_forbidden_words(forbidden_phrases, requirement)
    return "\n".join([f"{w}: {c}" for w, c in res]) if res else ""


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
    return (
        f"Flesch Reading Ease: {reading_ease_score:.2f}"
        if (
            # Adjust the threshold as needed
                reading_ease_score
                < DEFAULT_MINIMUM_REQUIRED_READING_SCORE
        )
        else ""
    )


def check_subjectivity_rule(requirement):
    # Rule 3: Check for subjectivity
    blob = TextBlob(requirement)
    subjectivity_score = blob.sentiment.subjectivity
    return (
        f"Subjectivity Score: {subjectivity_score:.2f}"
        if (
            # Adjust the threshold as needed
                subjectivity_score
                > DEFAULT_MAXIMUM_ALLOWED_SUBJECTIVITY_SCORE
        )
        else ""
    )


def check_is_requirement_rule(requirement):
    # Rule 4: Check if requirement text contains requirement keywords
    return not contains_any_keywords(requirement, requirement_keywords)


def check_security_related_rule(requirement):
    # Rule 5: Check if requirement is security-related
    return contains_any_keywords(requirement, security_keywords)


SMELL_DATA_HEADERS = [
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
smell_rules = zip(SMELL_DATA_HEADERS, all_rules_functions)

metrics_checks = [("Optionality", partial(count_forbidden_words, optionality_keywords)),
                  ("Conjunction", partial(count_forbidden_words, conjunction_keywords)),
                  ("Continuances", partial(count_forbidden_words, continuances_keywords)),
                  ("Imperatives", partial(count_forbidden_words, imperatives_keywords)),
                  ("Weakness", partial(count_forbidden_words, weakness_keywords)),
                  ("Vagueness", partial(count_forbidden_words, vagueness_keywords)),
                  ("Subjectivity", partial(count_forbidden_words, subjectivity_keywords)),
                  ("References", partial(count_forbidden_words, references_keywords))]

all_rule_checks = []
all_rule_checks.extend(smell_rules)
all_rule_checks.extend(metrics_checks)


def apply_rules(rule_list, smell_entry):
    for header, rule_check in rule_list:
        violates_rule_message = rule_check(smell_entry["Requirement"])
        if violates_rule_message:
            smell_entry[header] = violates_rule_message
    return smell_entry


def apply_smell_rules(smell_entry):
    return apply_rules(smell_rules, smell_entry)


def apply_metrics_rules(smell_entry):
    return apply_rules(metrics_checks, smell_entry)


def apply_all_rules(smell_entry):
    return apply_rules(all_rule_checks, smell_entry)
