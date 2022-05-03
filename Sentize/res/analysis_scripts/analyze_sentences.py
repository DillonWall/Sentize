# Make sure to pip install the following to your python distribution that is being used in the App.config
#pip install protobuf==3.14.0
#pip install transformers
#pip install onnxruntime

import torch
import torch.nn.functional as F
import onnxruntime
from transformers import AutoTokenizer

MODEL_NAME = 'bert-base-uncased'
MAX_LEN = 68

class Analyzer:
  def __init__(self, filepath):
    self.tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME)
    # onnx_model = onnx.load(filepath)
    # onnx.checker.check_model(onnx_model)
    self.ort_session = onnxruntime.InferenceSession(filepath)


  def tokenize(self, sentence):    
    #Preprocess the text to be suitable for the transformer
    tokens = self.tokenizer.tokenize(sentence) 
    tokens = ['[CLS]'] + tokens + ['[SEP]'] 
    if len(tokens) < MAX_LEN:
        tokens = tokens + ['[PAD]' for _ in range(MAX_LEN - len(tokens))] 
    else:
        tokens = tokens[:MAX_LEN-1] + ['[SEP]'] 
    #Obtain the indices of the tokens in the BERT Vocabulary
    input_ids = self.tokenizer.convert_tokens_to_ids(tokens) 
    input_ids = torch.tensor(input_ids) 
    #Obtain the attention mask i.e a tensor containing 1s for no padded tokens and 0s for padded ones
    attention_mask = (input_ids != 0).long()

    return input_ids, attention_mask


  def to_numpy(self, tensor):
    return tensor.detach().cpu().numpy() if tensor.requires_grad else tensor.cpu().numpy()


  def predict(self, input_ids, attention_mask): 
    # compute ONNX Runtime output prediction
    ort_inputs = {self.ort_session.get_inputs()[0].name: input_ids, 
                  self.ort_session.get_inputs()[1].name: attention_mask}
    logits = self.ort_session.run(None, ort_inputs)      
    
    probs = F.softmax(torch.tensor(logits[0]), dim=1)
    output = torch.argmax(probs, dim=1)

    return str(output.item()) + ':' + str(probs[0][output.item()].item())


  def analyze_sentence(self, sentence):
    x = self.tokenize(sentence)    
    input_ids = self.to_numpy(x[0].unsqueeze(0))
    attention_mask = self.to_numpy(x[1].unsqueeze(0))

    return self.predict(input_ids, attention_mask)


  def analyze_sentences(self, sentences):
    outputs = []
    for s in sentences:
      outputs.append(self.analyze_sentence(s))

    return outputs


# +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

import sys, warnings

# Suppress all kinds of warnings (this would lead to an exception on the client side).
warnings.simplefilter("ignore")

# parse command line arguments
filepath = sys.argv[1]
sentences = list(sys.argv[2].split('|'))

# Output all results
analyzer = Analyzer(filepath)
print(analyzer.analyze_sentences(sentences))

raise SystemExit(0)