# NALABS: NAtural LAnguage Bad Smells detector
![image](https://user-images.githubusercontent.com/7644735/145826101-d9ab2ed6-022c-4468-ae0a-7ef4880b05c1.png)

# Reference 

NALABS: Detecting Bad Smells in Natural Language Requirements and Test Specifications, Kostadin Rajkovic, Eduard Paul Enoiu
Report - MRTC, Mälardalen Real-Time Research Centre, Mälardalen University ISRN: MDH-MRTC-340/2022-1-SE

[Report] (http://www.es.mdh.se/publications/6382-NALABS__Detecting_Bad_Smells_in_Natural_Language_Requirements_and_Test_Specifications)

[ArXiv] (https://arxiv.org/abs/2202.05641)

# NALABSpy: Natural Language Analysis of Bad Smells in Software Requirements

NALABS is a Python script designed to analyze software requirements and identify potential bad smells that may indicate issues in the requirement's quality or clarity. The script reads requirements from an Excel file, processes them using various linguistic techniques, and outputs an Excel file containing the detected bad smells for each requirement.

You can find it in the NALABSpy directory. 

## NALABSpy: Features

NALABS performs the following analysis on each requirement:

1. Detects ambiguous words
2. Measures readability using the Flesch Reading Ease score
3. Calculates subjectivity to identify potential weaknesses in the requirement
4. Ensures that the requirement contains specific keywords
5. Checks if the requirement is security-related

## NALABSpy: Dependencies

- pandas
- openpyxl
- spacy
- textstat
- textblob

Install the dependencies using the following command:
```
pip3 install pandas openpyxl spacy textstat textblob
```

## NALABSpy: Usage

Run the script using the following command:

```bash
python3 NALABS.py <input_file> <id_column> <text_column> <output_file>
```

## NALABSpy: GUI
To run NALABSpy through a GUI, the following command should be run to install the web frontend:
```
pip install streamlit st-annotated-text
```
Once the packages have been installed, the web server can be started using
```
streamlit run web_gui.py
```

# Summary of Measures Used in NALABS C# version

1. Number of words (NW) 
2. Number of vague phrases (NV) 
3. Number of conjunctions (NC) 
4. Number of reference documents (NRD1 and NRD2)
5. Optionality (OP) 
6. Subjectivity (NS) 
7. Weakness (WK)
8. Automated Readability Index 
9. Imperatives (NI1 and NI2) 
10. Continuances (CT)
and others...

##  NALABS C# version - Documentation, Download and Install - 
More details about NALABS can be found in  http://www.diva-portal.org/smash/record.jsf?pid=diva2%3A1332337&dswid=9711 

NALABS is composed of two main components: the GUI as the main program executable and the metrics used as proxy for bad smells. 

The latest release of the NALABS executable can be downloaded from GitHub on the releases page. Alternatively, it can be built from source code. You can use different methods to build an application: the Visual Studio IDE and the MSBuild command-line tools. 

Add the package Microsoft.Office.Interop.Excel using the NuGet Package Manager. 


#  NALABS C# version - Using NALABS
 First change some settings. Choose Edit/Settings menu tab. In the Excel view you should choose the REQ ID and Text column in the requirement excel document.
 
 To open a requirement excel file choose the File/Open menu tab
 
 Known bug: When opening a new excel document, please remove the Settings.xml in the Settings folder located in the same place as the executable. This needs to be done since the program might crash.

# Funding
NALABS has been funded by the European Union’s Horizon 2020 research and innovation program under grant agreement No. 957212 and by the Swedish Innovation Agency (Vinnova) through the SmartDelta project.

# License
NALABS's source code is released under the MIT license
