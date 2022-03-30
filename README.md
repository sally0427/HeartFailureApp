# Heart Failure App
Predicting whether a patient is likely to develop heart failure with the chest X-Ray image.
the system have two part:
1. HeartFailure_flask : Load Model and predict(Python).
Use python + Flask to create a server to use "HeartFailure_URL".
2. HeartFailure_URL : Controller and view(C#).
Use HeartFailure_flask to complete this UI for server, a UI for Heart Failure classificati

# Use
### Clone this github
```
git clone https://github.com/sally0427/HeartFailureApp.git
```
###  Put the onnx model and Certificate Key in the file and edit the model path.
**If your own model can use openvino and myriad, you can change the python file named "ONNX_classify_flask_openvino.py" and change the URL in HeartFailure_URL**

## HeartFailure_flask
### Install python package 
using in ./HeartFailure/HeartFailure_flask/ONNX_classify_flask.py
```
pip install -r requirements.txt
```

## HeartFailure_URL
### Active Certificate Key
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/1.png)
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/2.png)
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/3.png)
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/4.png)

## Install windows app in your PC with Certificate Key
./HeartFailure_URL/HeartFailure/AppPackages/HeartFailure_1.0.2.0_Test/Add-AppDevPackage.ps1，按右鍵使用powershell執行安裝
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/5.png)
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/6.png)
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/7.png)
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/8.png)
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/9.png)

## Edit bat file to your own path
Change the path where your install the windows app in your PC.
```bash
C:\"Program Files"\WindowsApps\78f0adba-62c6-4d20-8cde-ac5a99d61f40_1.0.2.0_x64__835jxghk0an7e\HeartFailure.exe
```
Change the path where your install the windows app in your PC.
```bash
"C:\Users\AAEON\AppData\Local\Programs\Python\Python38\python.exe" "C:\Users\AAEON\Desktop\HeartFailure\HeartFailure_flask\ONNX_classify_flask.py"
```

## Processing bash.bat file
start using UI/UX until waiting for cmd flask server success.
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/10.png)

## 成功執行之畫面
![image](https://github.com/sally0427/HeartFailureApp/blob/master/HeartFailure_URL/ReadMe/11.png)

## Reference
[Mircosoft](https://docs.microsoft.com/zh-tw/windows/msix/package/packaging-uwp-apps)

# Develop
## Training Heart Failure Model
1. Using pytorch to pretrain without pretrained weight
2. Convert the .pt model to .onnx model using pytorch_model2ONNX_model in another github project.