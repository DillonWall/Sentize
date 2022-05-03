# Sentize
Emotion Analysis application that uses a Neural Network to analyze and determine what emotion (out of Joy, Sadness, Anger, Fear, Love, and Surprise) a sentence conveys.

Made by Dillon Wall for his Senior Project at OIT

## Credits
PyRunner code used to run a Python application in C Sharp obtained from 
[Using Python Scripts from a C# Client (Including Plots and Images)](https://www.codeproject.com/Articles/5165602/Using-Python-Scripts-from-a-Csharp-Client-Includin)
by [Thomas Weller](https://www.codeproject.com/script/Membership/View.aspx?mid=5134831)

## Technologies Used
* WPF (C# and XML)
* Onnx (Python import/export)
* BERT Tokenizer and Pretrained model
* PythonRunner code (CPOL license) (See Credits above)

## Installation
* Clone the repo to your local machine
* Download the onnx model from my [Dropbox Link](https://www.dropbox.com/s/qsc3o1fv47njjhz/BERT_Emotion.onnx?dl=0) and place it under "/Sentize/res/models/BERT_Emotion.onnx"
* Download Python 3.7.9 and install PyTorch, OnnxRuntime, and Transformers via Windows Command Prompt with the following commands:
> pip install torch
> 
> pip install onnxruntime
> 
> pip install transformers

Note: I tried installing this using an Anaconda environment, but the program would crash with the 
SSLError: "Can't connect to HTTPS URL because the SSL module is not available." because Anaconda uses its own SSL libraries. 
Unfortunately, the only way I know to get around this is to use your local python install.

* Change the value of "pythonPath" in "/Sentize/App.config" to the path of your python installation's executable, for example:
![image](https://user-images.githubusercontent.com/49173127/166120747-76e64b9d-cd6e-48dc-a3be-cf0771bef73e.png)

* Currently, the first time you analyze a sentence the program will likely crash after downloading the necessary files for the
'bert-base-uncased' config file. If this happens, simply close and reopen the program and it should work correctly.

## Usage
The primary goal of the software is to analyze any number of sentences and get results about which emotion they each convey. 

There are three main tabs to use depending on how the user would like to input the sentences to the program.
1. The "Sentence Analysis" tab allows analysis of only one sentence at a time. The user may optionally select which emotion the program should predict.
2. The "File Analysis" tab allows analysis of a .txt or .csv file. The program uses comma and/or tab separation, so sentences analyzed using this method must first be cleaned of commas and tabs. Users must specify which columns the sentences and actual emotions are in (or -1 if there is no such column). Actual emotions are represented via a number (NONE = -1, JOY = 0, SADNESS = 1, ANGER = 2, FEAR = 3, LOVE = 4, SURPRISE = 5), and must be included to see additional statistics about how accurate the neural network was. After analysis, the user may save results to a similarly named tab delimited txt file.
3. The "Database Analysis" tab allows analysis of an entire table in an SQL database. Users must enter the required information to connect to their database and table of choice, specifying the column names which contain the sentences to analyze and optionally their expected emotion. The user may save the results to the same table analyzed under the column "predicted_emotion". If this column is not already created, it will attempt to create it before saving.

Note: The "Send Feedback" button's functionality has been removed in the public release due to privacy concerns where it would add data to a table in my personal database. I may re-add this functionality in the future, but I would need to use a new secure method that does not expose any connection strings, etc.

## Status
This project is finished.

If I were to continue this project, I would like to improve the current NN model/accuracy (balance dataset)

Also, I would add versatility (accommodate different models with different inputs/outputs)

## Screenshots
Subsystem (context) Diagram
![image](https://user-images.githubusercontent.com/49173127/166392374-26373e3a-c7d3-44a3-9ff8-4fe0bd2e3bab.png)

Executive class diagram
![image](https://user-images.githubusercontent.com/49173127/166392405-dd9b3427-5983-4323-8a21-fcdbe0e8fb28.png)

![image](https://user-images.githubusercontent.com/49173127/166393150-2a19d5c3-5596-4b87-9046-2bff605fe5ac.png)

![image](https://user-images.githubusercontent.com/49173127/166393154-72f88cd2-8f5c-4827-a54c-7f413a2ab76d.png)

![image](https://user-images.githubusercontent.com/49173127/166393156-a46a7f4a-8033-451a-bdac-3d0f86ffaea6.png)

![image](https://user-images.githubusercontent.com/49173127/166586500-27cfc236-1b07-43d3-a9b4-685a26502bbb.png)

![image](https://user-images.githubusercontent.com/49173127/166585224-4d7f4045-f8cf-45cd-97c1-ffe1c10fc656.png)

![image](https://user-images.githubusercontent.com/49173127/166393162-b00a3c20-575d-4bf0-8d20-137d3b9d31ad.png)

