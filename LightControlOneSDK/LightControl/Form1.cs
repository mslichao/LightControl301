using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;

namespace LightControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Load("LightOff.png");
        }

        // 设置语言理解服务终结点、密钥、应用程序ID
        const string luisRegion = "******";
        const string luisKey = "********************************";
        const string luisAppId = "********-****-****-****-************";

        // 意图识别器
        IntentRecognizer recognizer;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                SpeechFactory speechFactory = SpeechFactory.FromSubscription(luisKey, "");
                recognizer = speechFactory.CreateIntentRecognizer("zh-cn");

                // 创建意图识别器用到的模型
                var model = LanguageUnderstandingModel.FromSubscription(luisKey, luisAppId, luisRegion);

                // 将模型中的意图加入到意图识别器中
                recognizer.AddIntent("None", model, "None");
                recognizer.AddIntent("TurnOn", model, "TurnOn");
                recognizer.AddIntent("TurnOff", model, "TurnOff");

                // 挂载识别中的事件
                // 收到中间结果
                recognizer.IntermediateResultReceived += Recognizer_IntermediateResultReceived;
                // 收到最终结果
                recognizer.FinalResultReceived += Recognizer_FinalResultReceived;
                // 发生错误
                recognizer.RecognitionErrorRaised += Recognizer_RecognitionErrorRaised;

                // 启动语音识别器，开始持续监听音频输入
                recognizer.StartContinuousRecognitionAsync();
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        // 识别过程中的中间结果
        private void Recognizer_IntermediateResultReceived(object sender, IntentRecognitionResultEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Result.Text))
            {
                Log("中间结果: " + e.Result.Text);
            }
        }

        // 出错时的处理
        private void Recognizer_RecognitionErrorRaised(object sender, RecognitionErrorEventArgs e)
        {
            Log("识别错误: " + e.FailureReason);
        }

        // 获得音频分析后的文本内容
        private void Recognizer_FinalResultReceived(object sender, IntentRecognitionResultEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Result.IntentId))
            {
                Log("最终语音转文本结果: " + e.Result.Text);

                string intent = e.Result.IntentId;
                Log("意图: " + intent + "\r\n");

                // 按照意图控制灯
                if (!string.IsNullOrEmpty(intent))
                {
                    if (intent.Equals("TurnOn", StringComparison.OrdinalIgnoreCase))
                    {
                        OpenLight();
                    }
                    else if (intent.Equals("TurnOff", StringComparison.OrdinalIgnoreCase))
                    {
                        CloseLight();
                    }
                }
            }
        }

        #region 界面操作

        private void Log(string message, params string[] parameters)
        {
            MakesureRunInUI(() =>
            {
                if (parameters != null && parameters.Length > 0)
                {
                    message = string.Format(message + "\r\n", parameters);
                }
                else
                {
                    message += "\r\n";
                }
                textBox1.AppendText(message);
            });
        }

        private void OpenLight()
        {
            MakesureRunInUI(() =>
            {
                pictureBox1.Load("LightOn.png");
            });
        }

        private void CloseLight()
        {
            MakesureRunInUI(() =>
            {
                pictureBox1.Load("LightOff.png");
            });
        }

        private void MakesureRunInUI(Action action)
        {
            if (InvokeRequired)
            {
                MethodInvoker method = new MethodInvoker(action);
                Invoke(action, null);
            }
            else
            {
                action();
            }
        }

        #endregion
    }
}
