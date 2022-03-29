import onnxruntime
import cv2
import numpy as np
import torchvision.transforms as transforms
import time
from flask import Flask, request
from openvino.inference_engine import IECore


def to_numpy(tensor):
    return tensor.detach().cpu().numpy() if tensor.requires_grad else tensor.cpu().numpy()

app = Flask(__name__)

@app.route('/', methods=['GET'])
def index():
    start = time.time()
    image_path = request.args.get('path')
    img = cv2.imread(image_path)
    img = cv2.resize(img, (512, 512), interpolation=cv2.INTER_AREA)
    to_tensor = transforms.ToTensor()
    img_y = to_tensor(img)
    img_y.unsqueeze_(0)

    # compute ONNX Runtime output prediction
    ort_inputs = {ort_session.get_inputs()[0].name: to_numpy(img_y)}
    ort_outs = ort_session.run(None, ort_inputs)
    img_out_y = ort_outs[0]
    index_max = np.argmax(img_out_y)
    print("output : ", ort_outs[0])
    end = time.time()
    print(f"Runtime of the program is {end - start}")
    return str(index_max)

@app.route('/openvino', methods=['GET'])
def openvino():
    start = time.time()
    image_path = request.args.get('path')
    img = cv2.imread(image_path)
    img = cv2.resize(img, (512, 512), interpolation=cv2.INTER_AREA)
    to_tensor = transforms.ToTensor()
    img_y = to_tensor(img)
    img_y.unsqueeze_(0)

    # ---------------------------Step 7. Do inference----------------------------------------------------------------------
    # res = exec_net.infer(inputs={input_blob: to_numpy(img_y)})
    res = exec_net.infer(inputs={input_blob: img_y})
    img_out_y = res["output"]
    index_max = np.argmax(img_out_y)
    print("output : ", res["output"])
    end = time.time()
    print(f"Runtime of the program is {end - start}")
    return str(index_max)


if __name__ == "__main__":

    # Openvino init
    ie = IECore()
    net = ie.read_network(model="C://Users//AAEON//Desktop//HeartFailure//HeartFailure_flask//HeartFailureClassifier.onnx")
    # Get names of input and output blobs
    input_blob = next(iter(net.input_info))
    out_blob = next(iter(net.outputs))

    # Set input and output precision manually
    # net.input_info[input_blob].precision = 'U8'
    net.input_info[input_blob].precision = 'FP32'
    net.outputs[out_blob].precision = 'FP32'

    # ---------------------------Step 4. Loading model to the device-------------------------------------------------------
    # exec_net = ie.load_network(network=net, device_name='CPU')
    ie.set_config({'VPU_HW_STAGES_OPTIMIZATION': 'NO'}, "MYRIAD")
    exec_net = ie.load_network(network=net, device_name='MYRIAD')
    print("net.input_info[input_blob].precision", net.input_info[input_blob].precision)
    print("net.outputs[out_blob].precision", net.outputs[out_blob].precision)

    # onnxruntime init
    ort_session = onnxruntime.InferenceSession("C://Users//AAEON//Desktop//HeartFailure//HeartFailure_flask//HeartFailureClassifier.onnx")

    # app.run(host="0.0.0.0", port=80, debug=False)
    app.run(debug=False)

    print("Exported model has been tested with ONNXRuntime, and the result looks good!")