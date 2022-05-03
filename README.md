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

![image](https://user-images.githubusercontent.com/49173127/166393162-b00a3c20-575d-4bf0-8d20-137d3b9d31ad.png)

