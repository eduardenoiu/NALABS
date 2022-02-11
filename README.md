# NALABS: NAtural LAnguage Bad Smells detector
![image](https://user-images.githubusercontent.com/7644735/145826101-d9ab2ed6-022c-4468-ae0a-7ef4880b05c1.png)


# Summary of Measures Used in NALABS

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

#  Documentation, Download and Install 
More details about NALABS can be found in  http://www.diva-portal.org/smash/record.jsf?pid=diva2%3A1332337&dswid=9711 

NALABS is composed of two main components: the GUI as the main program executable and the metrics used as proxy for bad smells. 

The latest release of the NALABS executable can be downloaded from GitHub on the releases page. Alternatively, it can be built from source code. You can use different methods to build an application: the Visual Studio IDE and the MSBuild command-line tools. 

Add the package Microsoft.Office.Interop.Excel using the NuGet Package Manager. 


#  Using NALABS
 First change some settings. Choose Edit/Settings menu tab. In the Excel view you should choose the REQ ID and Text column in the requirement excel document.
 
 To open a requirement excel file choose the File/Open menu tab
 
 Known bug: When opening a new excel document, please remove the Settings.xml in the Settings folder located in the same place as the executable. This needs to be done since the program might crash.



# Funding
NALABS has been funded by Bombardier Transportation through a thesis project, by the European Union’s Horizon 2020 research and innovation program under grant agreement No. 957212 and by the Swedish Innovation Agency (Vinnova) through the XIVT project. This work was partially funded from the Electronic Component Systems for European Leadership Joint Undertaking under grant agreement No. 737494 and The Swedish Innovation Agency, Vinnova (MegaM@Rt2). 

# License
NALABS's source code is released under the MIT license
