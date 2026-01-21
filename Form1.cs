using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using BBMath.Core;
using BBMath.Configuration;
//using Microsoft.VisualBasic;
using BBMath.UI;

namespace BBMath.Application
{
    /// <summary>
    /// 主窗体 - 数学练习软件界面
    ///
    /// 重要提示：关于广告资源
    /// ========================
    /// 本软件在 tpAbout（"原作说明"）标签页中展示了原作者的商品和社交媒体信息。
    /// 这些资源通过 Windows Forms 设计器静态绑定，编译时嵌入为资源文件。
    ///
    /// 受 MPL 2.0 协议保护，必须保留的内容：
    /// - pbOriProductMainPic: 商品主图（智能坐姿提醒器）
    /// - pbOriTaoBaoQCode: 淘宝商品链接二维码
    /// - pbOriWechatQCode: 微信公众号二维码
    /// - pbOriTiktokChinaQCode: 抖音账号二维码
    /// - pbOriRednoteQCode: 小红书账号二维码
    /// - tbOriInstruct1: 商品说明文本
    /// - tbOriOpenSrcDeclare: 开源协议声明文本
    /// - 相关的社交媒体标签文本框（tbTaoBao, tbWechat, tbTiktokChina, tbRednote）
    ///
    /// 详细资源版权声明请参见：Resources/README.txt
    ///
    /// 警告：请勿删除、隐藏或替换上述控件及资源，否则违反 MPL 2.0 协议。
    /// </summary>
    public partial class Form1 : BBMath.UI.BaseForm, BBMath.UI.IMainFormView
    {
        /// <summary>
        /// Presenter - 负责业务逻辑
        /// </summary>
        private MainFormPresenter _presenter;

        /// <summary>
        /// 0-题数框；1-答题框；2-辅助框
        /// </summary>
        int flagTB = 0;
        /// <summary>
        /// 当前的题目种类
        /// </summary>
        //int examType;                
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        /// <summary>
        /// 用作记录余数
        /// </summary>
        int numHalfResult = 0;
        ///// <summary>
        ///// 生成算式时计算的前两个数的得数，或用作储存余数
        ///// </summary>
        //int numHalfResult = 0;		
        int result = 0;
        /// <summary>
        /// 存放算式文本
        /// </summary>
        string strEquation;
        Random rd = new Random();
        DateTime startTime;
        /// <summary>
        /// 标记开始做题，用于保存
        /// </summary>
        bool startFlag = false;
        /// <summary>
        /// 已保存过的标记
        /// </summary>
        bool saveFlag = false;
        /// <summary>
        /// 计时倒数变量
        /// </summary>
        int counterTimeOut;
        public void selectTB()
        {
            switch (flagTB)
            {
                case 0:
                    tbTtlExamToDo.Select();
                    break;
                case 1:
                    tbResult.Select();
                    break;
                case 2:
                    tbRemainder.Select();
                    break;
            }
        }
        /// <summary>
        /// 答题
        /// </summary>
        public void answer()
        {
            GameStateManager.flagPause = false;
            int tmpResult = 0;
            int tmpRemainder = 0;

            bool resultForCheck = int.TryParse(tbResult.Text, out tmpResult);
            bool remainderForCheck = int.TryParse(tbRemainder.Text, out tmpRemainder);
            bool textBoxCheckOK = false;
            if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
            {
                //有余数除法的话要检查两个数的输入
                textBoxCheckOK = resultForCheck && remainderForCheck;
            }
            else
            {
                textBoxCheckOK = resultForCheck;
            }

            if (textBoxCheckOK)			//检测是否数字
            {
                timer1.Stop();
                //记录该题时间
                GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].ElapsedTime += GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TimeLimit - counterTimeOut;
                //答题结果检查
                if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
                {
                    //检查有余数除法结果
                    resultForCheck = (tmpResult == result) && (tmpRemainder == numHalfResult);
                }
                else
                {
                    //检查其他题型结果
                    resultForCheck = (tmpResult == result);
                }

                if (resultForCheck)                    //检查是否正确
                {
                    LoggerHelper.Print("检查结果：正确\r\n");
                    ExamObjQtyUpdate(true);
                    string tmp1;
                    if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)              //除法的完整算式记录
                    {
                        tmp1 = strEquation + " = " + result.ToString() + "......" + numHalfResult.ToString() + "\r\n";
                    }
                    else                            //其他的完整算式记录
                    {
                        tmp1 = strEquation + " = " + result.ToString() + "\r\n";
                    }
                    tbCorrect.AppendText(tmp1);
                    //检查做完没有
                    if (GameStateManager.examTtl <= 0)
                    {
                        //更新显示统计
                        UpdateDisp();
                        btnSubmitAnswer.Enabled = false;
                        tbResult.Enabled = false;
                        btnCheckAux.Enabled = false;
                        //btnGiveAux.Enabled = false;
                        btnPause.Enabled = false;
                        this.ControlBox = true;
                        DateTime completeTime = DateTime.Now;
                        TimeSpan span = completeTime.Subtract(startTime);
                        lbMin.Text = span.Minutes.ToString();
                        lbSec.Text = span.Seconds.ToString();
                        lbHour.Text = span.Hours.ToString();
                        tbRemainder.Visible = false;
                        double avgCost = span.TotalSeconds / (GameStateManager.correct + GameStateManager.wrong);
                        lbAvgTimeCost.Text = avgCost.ToString("f2");
                        //全对奖励金币（题目数量的1/2，向下取整）
                        if (GameStateManager.wrong == 0)
                        {
                            int bonus = (int)(GameStateManager.examTtlRec * AppConstants.FullAnswerBonusCoefficient);
                            MessageBox.Show("由于没有错误，所以额外奖励金币：" + bonus.ToString());
                            GameStateManager.coinTtl += bonus;
                            lbCoinTTL.Text = GameStateManager.coinTtl.ToString();
                        }
                        //保存
                        save();
                        saveFlag = true;
                        GameStateManager.finished = true;

                        // 显示"再练一次"按钮
                        btnDoAgain.Visible = true;
                    }
                    else                //未做完
                    {
                        LoggerHelper.Debug($"答题正确，currentTypeIndex={GameStateManager.currentTypeIndex}。开始生成新题目...");
                        // 使用Presenter生成新题目
                        _presenter.GenerateProblem();
                        tbResult.Text = "";
                        tbResult.Select();
                        // 如果是有余数除法，清空余数输入框并重置颜色
                        if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
                        {
                            tbRemainder.Text = "";
                            helpBoxColor();
                        }
                        //更新显示统计
                        UpdateDisp();
                        LoggerHelper.Debug($"界面更新完成，新题型 currentTypeIndex={GameStateManager.currentTypeIndex}");
                    }

                }
                else                                        //不正确
                {
                    ExamObjQtyUpdate(false);
                    string tmp1;
                    if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)              //除法的完整算式记录
                    {
                        tmp1 = strEquation + " = " + tbResult.Text + "余" + tbRemainder.Text +
                                " ( " + result.ToString() + "余" + numHalfResult.ToString() + " )" + "\r\n";
                    }
                    else                            //其他的完整算式记录
                    {
                        tmp1 = strEquation + " = " + tbResult.Text + " ( " + result.ToString() + " )" + "\r\n";
                    }

                    //string tmp1 = strEquation + " = " + tbResult.Text+" ( "+result.ToString()+ " )"+"\n";
                    tbRecord.AppendText(tmp1);

                    using (FormMsg frm = new FormMsg())
                    {
                        string tmp2;

                        if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
                        {
                            tmp2 = strEquation + " = " + tbResult.Text + "余" + tbRemainder.Text +
                                " (正确答案：  " + result.ToString() + "余" + numHalfResult.ToString() + " )" + "\n";
                        }
                        else
                        {
                            tmp2 = strEquation + " = " + tbResult.Text + " (正确答案：  " + result.ToString() + " )";
                        }
                        frm.label1.Text = tmp2;
                        frm.ShowDialog();
                    }
                    tbResult.Text = "";
                    // 使用Presenter生成新题目
                    _presenter.GenerateProblem();
                    tbResult.Select();
                    // 如果是有余数除法，清空余数输入框并重置颜色
                    if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
                    {
                        tbRemainder.Text = "";
                        helpBoxColor();
                    }
                    //更新显示统计
                    UpdateDisp();
                }

            }
            else
            {
                tbResult.Text = "";
                tbResult.Select();
            }
        }
        /// <summary>
        /// 根据各项数据更新控件显示
        /// </summary>
        public void UpdateDisp()
        {
            //MessageBox.Show("UpdateDisp()...");
            tbTtlExamToDo.Text = GameStateManager.examTtl.ToString(); // 使用总剩余题目数，而非各题型的剩余数
            textBox2.Text = GameStateManager.correct.ToString();
            textBox3.Text = GameStateManager.wrong.ToString();
            lbCoinTTL.Text = GameStateManager.coinTtl.ToString();
            pbTimeOut.Maximum = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TimeLimit;
            pbTimeOut.Value = counterTimeOut;
            //暂停可用信息显示
            if (GameStateManager.GetPauseTypeEnum() == PauseType.ByCount)
            {
                lbAllowPause.Text = GameStateManager.allowPause.ToString();
                lbPauseUnit.Text = Core.AppConstants.PauseUnitCount;
            }
            else if (GameStateManager.GetPauseTypeEnum() == PauseType.ByTime)
            {
                lbAllowPause.Text = GameStateManager.pauseSecLeft.ToString();
                lbPauseUnit.Text = Core.AppConstants.PauseUnitTime;
            }
            //暂停按钮可用状态更新
            if (GameStateManager.flagPause == true)                         //处于暂停中
            {
                btnPause.Enabled = false;
            }
            else if ((GameStateManager.allowPause > 0) && (GameStateManager.pauseType == 0))    //暂停计次，可用
            {
                btnPause.Enabled = true;
            }
            else if ((GameStateManager.pauseSecLeft > 0) && (GameStateManager.pauseType == 1)) //暂停计时，可用
            {
                btnPause.Enabled = true;
            }
            else                                                //没有可用暂停
            {
                btnPause.Enabled = false;
            }

            // 设置辅助框按钮状态（只在有余数除法时才启用辅助功能）
            if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
            {
                if (GameStateManager.helpBoxFree == true)
                {
                    btnCheckAux.Enabled = false;
                }
                else
                {
                    if (GameStateManager.coinTtl < GameStateManager.costCoinCheck)
                    {
                        btnCheckAux.Enabled = false;
                    }
                    else
                    {
                        //btnCheckAux.Enabled = true;
                        btnCheckAux.Enabled = false;//取消使用，这里手工设为隐藏
                    }
                }
            }
            else
            {
                btnCheckAux.Enabled = false;
            }

            // 余数框标签和文本框显示逻辑：
            // 只在当前题型是有余数除法时显示，其他情况全部隐藏
            bool showLabel = false;
            bool showTextBox = false;

            // 获取当前题型
            ExamType currentExamType = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype;
            string examDescription = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Description;

            if (currentExamType == ExamType.DivisionWithRemainder)
            {
                // 有余数除法：标签和文本框都显示
                showLabel = true;
                showTextBox = true;
                lbRemainder.Text = Core.AppConstants.RemainderLabel;
                LoggerHelper.Debug($"UpdateDisp: 有余数除法, 显示余数输入框. currentTypeIndex={GameStateManager.currentTypeIndex}, 题型={currentExamType}, 描述={examDescription}");
            }
            else
            {
                // 非有余数除法：全部隐藏
                showLabel = false;
                showTextBox = false;
                LoggerHelper.Debug($"UpdateDisp: 非有余数除法, 隐藏余数输入框. currentTypeIndex={GameStateManager.currentTypeIndex}, 题型={currentExamType}, 描述={examDescription}");
            }

            // 应用显示设置
            lbRemainder.Visible = showLabel;
            tbRemainder.Visible = showTextBox;
        }
        /// <summary>
        /// 根据正确或错误，具体题型的数量更新
        /// </summary>
        /// <param name="correct">是否正确</param>
        public void ExamObjQtyUpdate(bool correct)
        {
            if (correct)
            {
                GameStateManager.examTtl -= 1;
                //GameStateManager.examTypeQty[examtype] -= 1;
                GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TotalQty -= 1;
                GameStateManager.correct += 1;
                GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].CorrectQty += 1;
                GameStateManager.coinTtl += GameStateManager.awardCoin;
                // 使用总剩余题目数（examTtl）而非题型剩余数（TotalQty），因为新的题型池机制下 TotalQty 不再准确
                LoggerHelper.Print("正确，type=" + GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Description +
                    "\t总剩余=" + GameStateManager.examTtl.ToString() + "\r\n");
            }
            else
            {
                GameStateManager.examTtl += GameStateManager.punishment;
                GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TotalQty += GameStateManager.punishment;
                GameStateManager.wrong += 1;
                GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].WrongQty += 1;
                // 使用总剩余题目数（examTtl）而非题型剩余数（TotalQty），因为新的题型池机制下 TotalQty 不再准确
                LoggerHelper.Print("答题错误！type=" + GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Description +
                    "\t总剩余=" + GameStateManager.examTtl.ToString() + "\r\n");

                // 重新初始化题型池，确保题型分布均匀
                _presenter.ReinitializeExamTypePool();

            }
        }
        /// <summary>
        /// 出题和计算结果
        /// [Obsolete] 此方法已废弃，保留仅用于文档参考
        /// 现在题目生成由 MainFormPresenter.GenerateProblem() 负责
        /// 如果 Presenter 初始化失败，程序会终止，因此此方法永远不会被调用
        /// </summary>
        [Obsolete("请使用 Presenter.GenerateProblem() 代替", false)]
        public void GenNum()
        {
            // 获取当前难度配置
            var difficultyConfig = DifficultyConfigurationManager.GetConfig(GameStateManager.currentDifficulty);

            // 根据难度生成题目类型
            ExamType examType;
            if (difficultyConfig.SupportedOperations.Count > 0)
            {
                // 从难度支持的运算类型中随机选择
                int randomIndex = rd.Next(0, difficultyConfig.SupportedOperations.Count);
                examType = difficultyConfig.SupportedOperations[randomIndex];
                LoggerHelper.Debug($"随机选择题型: randomIndex={randomIndex}, examType={examType}");
            }
            else
            {
                // 如果没有支持的运算类型，使用默认逻辑
                do
                {
                    GameStateManager.currentTypeIndex = rd.Next(0, ExamObject.TotalTypeQty);
                } while (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TotalQty <= 0);
                examType = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype;
                LoggerHelper.Debug($"使用默认逻辑选择题型: currentTypeIndex={GameStateManager.currentTypeIndex}, examType={examType}");
            }

            // 同步当前题型索引到GameStateManager
            // 查找lstExamObjects中Examtype匹配的索引
            bool foundMatch = false;
            for (int i = 0; i < GameStateManager.lstExamObjects.Count; i++)
            {
                if (GameStateManager.lstExamObjects[i].Examtype == examType)
                {
                    GameStateManager.currentTypeIndex = i;
                    foundMatch = true;
                    LoggerHelper.Debug($"同步 currentTypeIndex: {i}, 题型: {examType}, 描述: {GameStateManager.lstExamObjects[i].Description}");
                    break;
                }
            }

            // 如果没有找到匹配，记录错误信息
            if (!foundMatch)
            {
                LoggerHelper.Error($"ERROR: 无法在 lstExamObjects 中找到题型 {examType}！");
                LoggerHelper.Error("lstExamObjects 当前列表：");
                for (int i = 0; i < GameStateManager.lstExamObjects.Count; i++)
                {
                    LoggerHelper.Error($"  [{i}] {GameStateManager.lstExamObjects[i].Description} - ExamType={GameStateManager.lstExamObjects[i].Examtype}");
                }
            }

            // 使用难度配置中的整数位数
            int currentIntBits = difficultyConfig.IntegerBits;

            LoggerHelper.Debug("生成题目难度：" + GameStateManager.currentDifficulty.ToString() +
                              "  题型：" + examType.ToString() +
                              "  整数位数：" + currentIntBits.ToString());

            // 根据题型和难度生成题目
            switch (examType)
            {
                case ExamType.Addition:		//加法
                    do
                    {
                        num1 = rd.Next(difficultyConfig.MinOperand, difficultyConfig.MaxOperand + 1);
                        num2 = rd.Next(difficultyConfig.MinOperand, difficultyConfig.MaxOperand + 1);
                        result = num1 + num2;
                        strEquation = num1.ToString() + " + " + num2.ToString();

                        // LV2难度：要求加法结果<100
                        if (GameStateManager.currentDifficulty == Difficulty.LV2)
                        {
                            if (result >= 100)
                            {
                                continue; // 重新生成
                            }
                        }
                        // LV4/LV5难度：结果可以超过100
                        break;
                    } while (GameStateManager.currentDifficulty == Difficulty.LV2 && result >= 100);
                    LoggerHelper.Print("Addition:" + num1.ToString() + " + " + num2.ToString() + " = " + result.ToString() + "\r\n");
                    break;

                case ExamType.Subtraction:	//减法
                    do
                    {
                        num1 = rd.Next(difficultyConfig.MinOperand, difficultyConfig.MaxOperand + 1);
                        num2 = rd.Next(difficultyConfig.MinOperand, difficultyConfig.MaxOperand + 1);
                        result = num1 - num2;
                        strEquation = num1.ToString() + " - " + num2.ToString();

                        // 检查是否允许负数（所有难度都不允许）
                        if (result < 0)
                        {
                            continue; // 重新生成
                        }

                        // LV2难度：要求减法结果<100
                        if (GameStateManager.currentDifficulty == Difficulty.LV2)
                        {
                            if (result >= 100)
                            {
                                continue; // 重新生成
                            }
                        }
                        break;
                    } while (true);
                    LoggerHelper.Print("Subtraction:" + num1.ToString() + " - " + num2.ToString() + " = " + result.ToString() + "\r\n");
                    break;

                case ExamType.Multiplication:   //乘法
                    do
                    {
                        num1 = rd.Next(difficultyConfig.MinOperand, difficultyConfig.MaxOperand + 1);
                        num2 = rd.Next(difficultyConfig.MinOperand, difficultyConfig.MaxOperand + 1);
                        result = num1 * num2;
                        strEquation = num1.ToString() + " * " + num2.ToString();
                        break;
                    } while (true);
                    LoggerHelper.Print("Multiplication:" + num1.ToString() + " ， " + num2.ToString() + " = " + result.ToString() + "\r\n");
                    break;

                case ExamType.DivisionWithRemainder://有余数除法
                    do
                    {
                        ///		 A / B = C ...D
                        num2 = rd.Next(2, difficultyConfig.MaxOperand + 1);               //B 除数
                        result = rd.Next(1, difficultyConfig.MaxOperand + 1);             //C 得数
                        numHalfResult = rd.Next(1, num2);		//D 余数（必须小于除数）
                        num1 = num2 * result + numHalfResult;	//A 被除数
                        strEquation = num1.ToString() + " ÷ " + num2.ToString();

                        // LV3/LV4难度：要求被除数<100
                        if (GameStateManager.currentDifficulty == Difficulty.LV3 || GameStateManager.currentDifficulty == Difficulty.LV4)
                        {
                            if (num1 >= 100)
                            {
                                continue; // 重新生成
                            }
                        }
                        // LV5难度：要求被除数>=100
                        if (GameStateManager.currentDifficulty == Difficulty.LV5)
                        {
                            if (num1 < 100)
                            {
                                continue; // 重新生成
                            }
                        }
                        break;
                    } while (true);
                    LoggerHelper.Print("DivisionWithRemainder:" + num1.ToString() + " ÷ " + num2.ToString() + " = "
                                + result.ToString() + "......" + numHalfResult.ToString() + "\r\n");
                    break;

                case ExamType.DivisionNoRemainder://无余数除法
                    do
                    {
                        ///		 A / B = C
                        num2 = rd.Next(1, difficultyConfig.MaxOperand + 1);               //B 除数
                        result = rd.Next(1, difficultyConfig.MaxOperand + 1);             //C 得数
                        num1 = num2 * result;   //A 被除数（保证整除）
                        strEquation = num1.ToString() + " ÷ " + num2.ToString();

                        // LV3/LV4难度：要求被除数<100
                        if (GameStateManager.currentDifficulty == Difficulty.LV3 || GameStateManager.currentDifficulty == Difficulty.LV4)
                        {
                            if (num1 >= 100)
                            {
                                continue; // 重新生成
                            }
                        }
                        // LV5难度：要求被除数>=100
                        if (GameStateManager.currentDifficulty == Difficulty.LV5)
                        {
                            if (num1 < 100)
                            {
                                continue; // 重新生成
                            }
                        }
                        break;
                    } while (true);
                    LoggerHelper.Print("DivisionNoRemainder:" + num1.ToString() + " ÷ " + num2.ToString() + " = "
                                + result.ToString() + "\r\n");
                    break;
            }
            //-----------------------------------------
            equation.Text = strEquation;
            // 设置题目时间限制（使用默认值或从配置读取）
            counterTimeOut = 30; // 默认30秒
            // 也可以尝试从 GameStateManager 获取对应题型的时间限制
            if (GameStateManager.lstExamObjects != null && GameStateManager.lstExamObjects.Count > 0)
            {
                // 查找对应题型的 ExamObject
                var examObj = GameStateManager.lstExamObjects.FirstOrDefault(obj => obj.Examtype == examType);
                if (examObj != null && examObj.TimeLimit > 0)
                {
                    counterTimeOut = examObj.TimeLimit;
                }
            }

            timer1.Start();

            // 记录最终状态
            LoggerHelper.Debug($"GenNum 完成. examType={examType}, currentTypeIndex={GameStateManager.currentTypeIndex}, strEquation={strEquation}");
        }
        public void save()
        {
            try
            {
                if (!startFlag) return;
                if (saveFlag) return;

                DateTime completeTime = DateTime.Now;
                TimeSpan span = completeTime.Subtract(startTime);

                double avgCost = span.TotalSeconds / (GameStateManager.correct + GameStateManager.wrong);
                //汇总信息
                string path = AppDomain.CurrentDomain.BaseDirectory + GameStateManager.fileName;
                // 获取难度配置的显示文本
                var difficultyConfig = DifficultyConfigurationManager.GetConfig(GameStateManager.currentDifficulty);
                string difficultyText = difficultyConfig.DisplayText;

                string strLog = "[记录]  开始时间: " + startTime.ToString() + "\r\n" +
                                " 难度: " + difficultyText + "\r\n" +
                                " 正确: " + GameStateManager.correct.ToString() + "； 错误: " + GameStateManager.wrong.ToString() + "； 未完成: " + GameStateManager.examTtl.ToString() + "； 金币: " + GameStateManager.TotalCoin.ToString() + "\r\n";
                strLog += " 用时：" + span.Hours.ToString() + " 小时， " + span.Minutes.ToString() + " 分钟， " + span.Seconds.ToString() + " 秒  " + "平均" + avgCost.ToString("f2") + "秒/题\r\n";
                //题型信息打印
                foreach (ExamObject obj in GameStateManager.lstExamObjects)
                {
                    if (obj.SumQty > 0)
                    {
                        strLog += "<--题型-->   " + obj.Description + "  , \t剩余：" + obj.TotalQty.ToString() + ", 正确：" + obj.CorrectQty.ToString() + ", 错误：" + obj.WrongQty.ToString() + "\r\n";
                        strLog += "   耗时(秒)：" + obj.ElapsedTime.ToString() + " , 平均：" + ((double)obj.ElapsedTime / obj.SumQty).ToString("f2") + "\r\n";
                    }
                }
                //软件版本
                strLog += "------ 软件版本: Ver " + System.Windows.Forms.Application.ProductVersion.ToString() + " ------ \r\n";
                //记录到文件
                if (!File.Exists(path))
                {
                    FileStream stream = File.Create(path);
                    stream.Close();
                    stream.Dispose();
                }
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(strLog);
                }

                // 保存金币到配置文件
                GameStateManager.SaveProgSettings();
                LoggerHelper.Print($"金币已保存到配置文件: {GameStateManager.coinTtl}\r\n");
            }
            catch
            {

            }
        }
        public Form1()
        {
            InitializeComponent();
            InitializePresenter();
        }

        /// <summary>
        /// 初始化 Presenter
        /// 注意：这是关键初始化步骤，如果失败会导致程序无法正常运行
        /// </summary>
        private void InitializePresenter()
        {
            try
            {
                LoggerHelper.Info("开始初始化 Presenter...");

                // 步骤 1：创建日志服务
                LoggerService logger;
                try
                {
                    //throw new Exception("测试");// 临时测试
                    logger = new LoggerService();
                    LoggerHelper.Logger = logger;
                    LoggerHelper.Info("日志服务初始化成功");
                }
                catch (Exception ex)
                {
                    string errorMsg = $"日志服务初始化失败：{ex.Message}\n\n程序无法继续运行，即将退出。";
                    LoggerHelper.Error(errorMsg);
                    MessageBox.Show(errorMsg, "初始化失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // 注意：在窗体构造函数等早期阶段，Application.Exit() 可能不会立即生效
                    // 使用 Environment.Exit(0) 确保进程立即终止
                    System.Environment.Exit(1);  // 非零退出码表示异常
                    return;  // 这行不会执行，但保留以提高代码可读性
                }

                // 步骤 2：创建配置服务
                IConfigurationService configService;
                try
                {
                    configService = new IniConfigurationService();
                    LoggerHelper.Info("配置服务初始化成功");
                }
                catch (Exception ex)
                {
                    string errorMsg = $"配置服务初始化失败：{ex.Message}\n\n程序无法继续运行，即将退出。";
                    LoggerHelper.Error(errorMsg);
                    LoggerHelper.Exception(ex);
                    MessageBox.Show(errorMsg, "初始化失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(1);
                    return;
                }

                // 步骤 3：创建 Presenter
                try
                {
                    _presenter = new MainFormPresenter(this, logger, configService);
                    LoggerHelper.Info("Presenter 初始化成功");
                }
                catch (Exception ex)
                {
                    string errorMsg = $"Presenter 初始化失败：{ex.Message}\n\n程序无法继续运行，即将退出。";
                    LoggerHelper.Error(errorMsg);
                    LoggerHelper.Exception(ex);
                    MessageBox.Show(errorMsg, "初始化失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(1);
                    return;
                }

                LoggerHelper.Info("Presenter 初始化完成");
            }
            catch (Exception ex)
            {
                // 捕获未预期的异常
                string errorMsg = $"Presenter 初始化过程中发生未知错误：{ex.Message}\n\n程序无法继续运行，即将退出。";
                try
                {
                    LoggerHelper.Error(errorMsg);
                    LoggerHelper.Exception(ex);
                }
                catch
                {
                    // 如果日志系统也失败了，忽略
                }
                MessageBox.Show(errorMsg, "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                System.Environment.Exit(1);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            LoggerHelper.Print("程序开始\r\n");

            if (int.TryParse(tbTtlExamToDo.Text, out GameStateManager.examTtl))
            {
                timer2.Enabled = true;                          //进程检查定时器
                GameStateManager.examTtlRec = GameStateManager.examTtl;             //记录原始题数，用于无错奖励

                // 重置暂停时间和暂停次数为配置值（Configuration Value）
                // 将运行时值（Runtime Value）重置为固定的配置值
                GameStateManager.pauseSecLeft = GameStateManager.pauseSecondsLeftConfig;
                GameStateManager.allowPause = GameStateManager.allowPauseConfig;

                btnStart.Enabled = false;
                btnSubmitAnswer.Enabled = true;
                tbTtlExamToDo.Enabled = false;
                this.ControlBox = false;
                tbResult.Select();
                flagTB = 1;
                GameStateManager.UpdateExamQty();

                // 隐藏"再练一次"按钮
                btnDoAgain.Visible = false;

                // 初始化题型池（确保题型均匀分布）
                _presenter.InitializeExamTypePool();

                // 使用 Presenter 生成题目
                _presenter.GenerateProblem();
                // 设置倒计时时间并启动计时器
                counterTimeOut = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TimeLimit;
                timer1.Start();

                startTime = DateTime.Now;
                lbStartTime.Text = startTime.ToString();
                startFlag = true;
                label1.Text = "剩余";
                //更新控件显示
                UpdateDisp();
            }
            else
            {
                MessageBox.Show("总数必须输入数字", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbTtlExamToDo.Select();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 保持原有的answer()方法调用，确保所有功能正常
            answer();
        }

        private void tbResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2_Click(sender, e);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //标题栏显示程序版本（从 AssemblyVersion 读取）
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
            {
                this.Text = this.Text + version.ToString();
            }
            //初始化各项参数，从文件载入配置
            //MessageBox.Show("load()初始化前，List对象数:" + lstExamObjects.Count.ToString() + "静态变量数值：" + ExamObject.TotalTypeQty.ToString());
            GameStateManager.InitSettings();

            // 记录初始状态
            LoggerHelper.Debug($"Form1_Load: lstExamObjects.Count={GameStateManager.lstExamObjects.Count}");
            for (int i = 0; i < GameStateManager.lstExamObjects.Count; i++)
            {
                LoggerHelper.Debug($"  [{i}] {GameStateManager.lstExamObjects[i].Description} - ExamType={GameStateManager.lstExamObjects[i].Examtype}");
            }

            // 初始化难度选择控件
            InitializeDifficultyComboBox();

            // 确保"再练一次"按钮初始不可见
            btnDoAgain.Visible = false;

            // 设置默认题目总数
            tbTtlExamToDo.Text = AppConstants.DefaultExamTotal.ToString();

            UpdateDisp();
            btnCheckAux.Enabled = false;
            btnPause.Enabled = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("1");
        }

        private void but1_Leave(object sender, EventArgs e)
        {

        }

        private void but2_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("2");
        }

        private void but3_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("3");
        }

        private void but4_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("4");
        }

        private void but5_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("5");
        }

        private void but6_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("6");
        }

        private void but7_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("7");
        }

        private void but8_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("8");
        }

        private void but9_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("9");
        }

        private void but0_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("0");
        }

        private void butDot_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait(".");
        }

        private void butBS_Click(object sender, EventArgs e)
        {
            selectTB();
            SendKeys.SendWait("{BACKSPACE}");
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //save();
            formViewLog frVL = new formViewLog();
            //frVL.Show(this);
            frVL.ShowDialog();
            //frVL.ShowDialog(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameStateManager.saveProgSettings();
            save();

            // 使用 Presenter 清理资源
            _presenter.Cleanup();

            // 清理日志资源
            LoggerHelper.Shutdown();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("Closed!");
        }
        private void helpBoxColor()
        {
            tbRemainder.BackColor = Color.White;
        }
        private void helpBoxColor(int val)
        {
            int temptext;
            if (int.TryParse(tbRemainder.Text, out temptext))
            {
                if (temptext == val)
                {
                    tbRemainder.BackColor = Color.PaleGreen;
                }
                else
                {
                    tbRemainder.BackColor = Color.Tomato;
                }
            }
            else
            {
                tbRemainder.BackColor = Color.Tomato;
            }
        }
        private void checkHelpBox()
        {
            helpBoxColor(numHalfResult);

        }
        private void giveNumHelpBox()
        {
            //int temp;
            //switch (examType)
            //{
            //    case 0://加
            //    case 3:
            //        temp = num1 + num2;
            //        tbHelp.Text = temp.ToString();
            //        helpBoxColor(temp);
            //        break;
            //    case 1://减
            //    case 2:
            //        temp = num1 - num2;
            //        tbHelp.Text = temp.ToString();
            //        helpBoxColor(temp);
            //        break;
            //}
            tbRemainder.Text = numHalfResult.ToString();
            helpBoxColor(numHalfResult);

        }
        private void tbHelp_TextChanged(object sender, EventArgs e)
        {
            // 只在有余数除法时才执行辅助框逻辑
            if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
            {
                if (GameStateManager.helpBoxFree == true)        //免费使用
                {
                    checkHelpBox();
                }
                else
                {
                    helpBoxColor();
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            //MessageBox.Show("tb1 Enter!");
            flagTB = 0;
        }

        private void tbResult_Enter(object sender, EventArgs e)
        {
            flagTB = 1;
        }

        private void tbHelp_Enter(object sender, EventArgs e)
        {
            flagTB = 2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool timerStatus = timer1.Enabled;
            if (int.TryParse(tbTtlExamToDo.Text, out GameStateManager.examTtl))
            {
            }
            else
            {
                MessageBox.Show("总数必须输入数字", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbTtlExamToDo.Select();
                return;
            }
            string psw;
            bool allowChange = false;
            using (frmPsw frm = new frmPsw())
            {
                frm.ShowDialog();
                psw = frm.OutValue;
            }
            if (GameStateManager.PSW == psw)
            {
                allowChange = true;
                timer1.Stop();
            }
            using (frmSetup frm = new frmSetup(allowChange))
            {
                frm.ShowDialog();
            }
            if ((allowChange == true) && (timerStatus == true))
            {
                timer1.Start();
            }

            if (GameStateManager.finished == false)
            {
                UpdateDisp();
            }


        }

        private void label6_Validated(object sender, EventArgs e)
        {
            //MessageBox.Show("validated");
            //label6.Text = GameStateManager.coinPocket.ToString();
        }
        /// <summary>
		/// 检查数值按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button5_Click(object sender, EventArgs e)
        {
            int tmp;
            if (!(int.TryParse(tbRemainder.Text, out tmp)))
            {
                tbRemainder.Text = "";
                return;
            }
            GameStateManager.coinTtl -= GameStateManager.costCoinCheck;
            if (GameStateManager.coinTtl < 0)
            {
                GameStateManager.coinTtl = 0;
            }

            UpdateDisp();
            checkHelpBox();
            tbResult.Select();
        }
        /// <summary>
		/// 给中间值答案按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button6_Click(object sender, EventArgs e)
        {
            //GameStateManager.coinPocket -= GameStateManager.costCoinGive;
            //if (GameStateManager.coinPocket < 0)
            //{
            //    if (GameStateManager.coinBox > pbCoin.Maximum)
            //    {
            //        GameStateManager.coinPocket += pbCoin.Maximum;
            //        GameStateManager.coinBox -= pbCoin.Maximum;
            //    }
            //    else
            //    {
            //        GameStateManager.coinPocket += GameStateManager.coinBox;
            //        GameStateManager.coinBox = 0;
            //    }
            //    //GameStateManager.coinPocket = 0;
            //    //helpBoxColor();
            //    //MessageBox.Show("金币不足，无法计算");
            //    //return;
            //}
            GameStateManager.coinTtl -= GameStateManager.costCoinGive;
            if (GameStateManager.coinTtl < 0)
            {
                GameStateManager.coinTtl = 0;
            }
            UpdateDisp();
            giveNumHelpBox();
            tbResult.Select();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (counterTimeOut > 0)
            {
                counterTimeOut--;
            }
            else
            {
                counterTimeOut = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TimeLimit;
                GameStateManager.examTtl++;
                GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TotalQty += 1;

                // 重新初始化题型池，确保题型分布均匀
                _presenter.ReinitializeExamTypePool();

                UpdateDisp();
            }
            pbTimeOut.Value = counterTimeOut;
            lbTimeOut.Text = counterTimeOut.ToString();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _presenter.TogglePause();

            if (GameStateManager.GetPauseTypeEnum() == PauseType.ByCount)                               //限次数暂停
            {
                if (GameStateManager.allowPause > 0)
                {
                    timer1.Stop();
                    GameStateManager.flagPause = true;
                    GameStateManager.allowPause--;
                    using (FormPause frm = new FormPause())
                    {
                        frm.ShowDialog();
                    }
                    timer1.Start();
                    GameStateManager.flagPause = false;
                }
            }
            else if (GameStateManager.GetPauseTypeEnum() == PauseType.ByTime)                           //限时间暂停
            {
                if (GameStateManager.pauseSecLeft > 0)
                {
                    timer1.Stop();
                    GameStateManager.flagPause = true;
                    using (FormPause frm = new FormPause())
                    {
                        frm.ShowDialog();
                    }
                    timer1.Start();
                    GameStateManager.flagPause = false;
                }
            }
            UpdateDisp();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Process[] ps = Process.GetProcesses();
            System.Diagnostics.Debug.WriteLine("===== 进程检查 =====");
            System.Diagnostics.Debug.WriteLine($"Total: {ps.Length}");
            int cnt = 0;
            foreach (Process p in ps)
            {
                if (p.MainWindowHandle != null)
                {
                    cnt++;
                    if (p.MainWindowTitle != "")
                    {
                        System.Diagnostics.Debug.WriteLine($"[{cnt}] {p.MainWindowTitle} ({p.ProcessName})");
                    }
                    //kill
                    //if (p.ProcessName== "Calculator")
                    //{
                    //    p.Kill();
                    //}
                    //(SogouExplorer)
                    //if (p.ProcessName == "SogouExplorer")
                    //{
                    //    p.Kill();
                    //}
                }
            }
            System.Diagnostics.Debug.WriteLine("===== 检查完成 =====");
        }
        /// <summary>
        /// 杀常用的计算进程，暂时屏蔽掉实际执行语句，先这样保留着，待以后看情况再恢复。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                Process[] ps = Process.GetProcesses();
                foreach (Process p in ps)
                {
                    if (p.MainWindowHandle != null)
                    {
                        foreach (string stCheck in GameStateManager.strProcessBlackList)
                        {
                            if (p.ProcessName == stCheck)
                            {
                                //p.Kill();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Print("Kill 错误：" + ex.Message);
            }
        }

        #region IMainFormView 接口实现

        /// <summary>
        /// 显示题目
        /// </summary>
        public void DisplayProblem(string problemText)
        {
            equation.Text = problemText;
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        public void UpdateStatistics(int correct, int wrong, int remaining, int coins)
        {
            textBox2.Text = correct.ToString();
            textBox3.Text = wrong.ToString();
            tbTtlExamToDo.Text = remaining.ToString();
            lbCoinTTL.Text = coins.ToString();
        }

        /// <summary>
        /// 记录正确答案
        /// </summary>
        public void RecordCorrectAnswer(string equation, int result, int remainder)
        {
            string recordText;
            if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
            {
                recordText = equation + " = " + result.ToString() + "..." + remainder.ToString() + "\r\n";
            }
            else
            {
                recordText = equation + " = " + result.ToString() + "\r\n";
            }
            tbCorrect.AppendText(recordText);
        }

        /// <summary>
        /// 记录错误答案
        /// </summary>
        public void RecordWrongAnswer(string equation, int userResult, int userRemainder, int correctResult, int correctRemainder)
        {
            string recordText;
            if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
            {
                recordText = equation + " = " + userResult.ToString() + "余" + userRemainder.ToString() +
                            " ( " + correctResult.ToString() + "余" + correctRemainder.ToString() + " )" + "\r\n";
            }
            else
            {
                recordText = equation + " = " + userResult.ToString() + " ( " + correctResult.ToString() + " )" + "\r\n";
            }
            tbRecord.AppendText(recordText);
        }

        /// <summary>
        /// 显示错误答案对话框
        /// </summary>
        public void ShowErrorAnswerDialog(string equation, int userResult, int userRemainder, int correctResult, int correctRemainder)
        {
            using (FormMsg frm = new FormMsg())
            {
                string message;
                if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
                {
                    message = equation + " = " + userResult.ToString() + "余" + userRemainder.ToString() +
                            " (正确答案：  " + correctResult.ToString() + "余" + correctRemainder.ToString() + " )";
                }
                else
                {
                    message = equation + " = " + userResult.ToString() + " (正确答案：  " + correctResult.ToString() + " )";
                }

                frm.label1.Text = message;
                frm.ShowDialog();
            }
        }

        /// <summary>
        /// 清空答案输入框
        /// </summary>
        public void ClearAnswerInput()
        {
            tbResult.Text = "";
            // 如果是有余数除法，清空余数输入框并重置颜色
            if (GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype == ExamType.DivisionWithRemainder)
            {
                tbRemainder.Text = "";
                helpBoxColor();
            }
            tbResult.Select();
        }

        /// <summary>
        /// 禁用考试控件
        /// </summary>
        public void DisableExamControls()
        {
            btnSubmitAnswer.Enabled = false;
            tbResult.Enabled = false;
            btnCheckAux.Enabled = false;
            btnPause.Enabled = false;
            this.ControlBox = true;
            tbRemainder.Visible = false;
        }

        /// <summary>
        /// 更新完成时间
        /// </summary>
        public void UpdateCompletionTime()
        {
            DateTime completeTime = DateTime.Now;
            TimeSpan span = completeTime.Subtract(startTime);
            lbMin.Text = span.Minutes.ToString();
            lbSec.Text = span.Seconds.ToString();
            lbHour.Text = span.Hours.ToString();
        }

        /// <summary>
        /// 更新平均用时
        /// </summary>
        public void UpdateAverageTimeCost(int totalQuestions)
        {
            TimeSpan span = DateTime.Now.Subtract(startTime);
            double avgCost = span.TotalSeconds / totalQuestions;
            lbAvgTimeCost.Text = avgCost.ToString("f2");
        }

        /// <summary>
        /// 显示奖励对话框
        /// </summary>
        public void ShowBonusDialog(int bonus)
        {
            MessageBox.Show("由于没有错误，所以额外奖励金币：" + bonus.ToString(),
                            "奖励", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 更新题型显示
        /// </summary>
        public void UpdateExamTypeDisplay(int index)
        {
            // 当前题目题型由 Presenter.GenerateProblem() 随机生成
            // 如果需要更新题型显示，可以在这里实现
        }

        /// <summary>
        /// 更新暂停状态
        /// </summary>
        public void UpdatePauseState(bool isPaused)
        {
            // 暂停状态在 UpdateDisp() 中已经处理
            // 如果需要额外的更新，可以在这里实现
        }

        /// <summary>
        /// 更新金币显示
        /// </summary>
        public void UpdateCoinDisplay(int coins)
        {
            lbCoinTTL.Text = coins.ToString();
        }

        /// <summary>
        /// 显示/隐藏辅助框
        /// </summary>
        public void ShowHelperBox(bool show)
        {
            tbRemainder.Visible = show;
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        public void ShowError(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 从 Presenter 同步题目数据到视图
        /// </summary>
        public void SyncProblemData(int num1, int num2, int result, int numHalfResult, string equation)
        {
            this.num1 = num1;
            this.num2 = num2;
            this.result = result;
            this.numHalfResult = numHalfResult;
            this.strEquation = equation;
        }

        /// <summary>
        /// 重置答题定时器
        /// </summary>
        public void ResetTimer()
        {
            try
            {
                // 设置倒计时时间为当前题型的时间限制
                counterTimeOut = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TimeLimit;

                // 重启定时器
                timer1.Start();

                // 记录日志便于调试
                LoggerHelper.Debug($"定时器已重置. counterTimeOut={counterTimeOut}, 题型={GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].Examtype}");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"重置定时器失败：{ex.Message}");
                LoggerHelper.Exception(ex);
            }
        }

        /// <summary>
        /// 初始化难度选择下拉框
        /// </summary>
        private void InitializeDifficultyComboBox()
        {
            // 清空原有选项
            cbDifficultySelect.Items.Clear();

            // 动态添加5个难度选项
            cbDifficultySelect.Items.Add("(1)个位数加减法");
            cbDifficultySelect.Items.Add("(2)100以内加减法");
            cbDifficultySelect.Items.Add("(3)个位数乘除法");
            cbDifficultySelect.Items.Add("(4)100以内加减乘除带余数");
            cbDifficultySelect.Items.Add("(5)100以上加减乘除带余数");

            // 设置默认选中第一项（LV1难度）
            if (cbDifficultySelect.Items.Count > 0)
            {
                cbDifficultySelect.SelectedIndex = 0;
            }

            // 确保状态管理器中的难度同步
            GameStateManager.currentDifficulty = Difficulty.LV1;

            LoggerHelper.Debug($"难度选择初始化完成，默认难度: {Difficulty.LV1}");
        }

        /// <summary>
        /// 难度选择下拉框的选项改变事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDifficultySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 检查是否有选中的项
            if (cbDifficultySelect.SelectedIndex < 0 || cbDifficultySelect.SelectedIndex >= cbDifficultySelect.Items.Count)
            {
                return;
            }

            // 根据索引映射难度
            // 0: "(1)个位数加减法" -> LV1
            // 1: "(2)100以内加减法" -> LV2
            // 2: "(3)个位数乘除法" -> LV3
            // 3: "(4)100以内加减乘除带余数" -> LV4
            // 4: "(5)100以上加减乘除带余数" -> LV5
            Difficulty newDifficulty;
            string selectedText = cbDifficultySelect.SelectedItem.ToString();

            switch (cbDifficultySelect.SelectedIndex)
            {
                case 0:
                    newDifficulty = Difficulty.LV1;
                    break;
                case 1:
                    newDifficulty = Difficulty.LV2;
                    break;
                case 2:
                    newDifficulty = Difficulty.LV3;
                    break;
                case 3:
                    newDifficulty = Difficulty.LV4;
                    break;
                case 4:
                    newDifficulty = Difficulty.LV5;
                    break;
                default:
                    newDifficulty = Difficulty.LV1;
                    break;
            }

            // 更新状态管理器中的难度
            GameStateManager.currentDifficulty = newDifficulty;

            LoggerHelper.Debug($"难度已切换为: {newDifficulty} ({selectedText})");

            // 如果已经在答题过程中，提示用户下次答题生效
            if (startFlag && !saveFlag)
            {
                MessageBox.Show("难度将在下次答题时生效", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        private void btnDoAgain_Click(object sender, EventArgs e)
        {
            // 隐藏"再练一次"按钮
            btnDoAgain.Visible = false;

            // 重置题目数量输入框
            tbTtlExamToDo.Enabled = true;
            tbTtlExamToDo.Text = "";
            tbTtlExamToDo.Select();

            // 重置开始按钮
            btnStart.Enabled = true;
            btnSubmitAnswer.Enabled = false;

            // 重置答题结果框
            tbResult.Enabled = true;
            tbResult.Text = "";

            // 重置辅助框
            btnCheckAux.Enabled = false;
            tbRemainder.Text = "";
            tbRemainder.Visible = false;

            // 重置暂停按钮
            btnPause.Enabled = false;

            // 重置状态标志
            startFlag = false;
            saveFlag = false;
            GameStateManager.finished = false;

            // 重置统计显示
            GameStateManager.correct = 0;
            GameStateManager.wrong = 0;
            GameStateManager.examTtl = AppConstants.DefaultExamTotal;
            GameStateManager.examTtlRec = 0;

            // 重置暂停时间和暂停次数为配置值（Configuration Value）
            // 点击"再练一次"后立即重置运行时值（Runtime Value）
            GameStateManager.pauseSecLeft = GameStateManager.pauseSecondsLeftConfig;
            GameStateManager.allowPause = GameStateManager.allowPauseConfig;

            // 清空正确和错误记录框
            tbCorrect.Clear();
            tbRecord.Clear();

            // 重置时间显示
            lbMin.Text = "0";
            lbSec.Text = "0";
            lbHour.Text = "0";
            lbAvgTimeCost.Text = "0.00";
            lbStartTime.Text = "";

            // 重置标签显示
            label1.Text = "总数";

            // 更新显示
            UpdateDisp();

            LoggerHelper.Debug("已重置为初始状态，可以重新开始答题");
        }

		private void llbTaoBao_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            llbTaoBao.LinkVisited = true;
            Process.Start("IExplore", "https://item.taobao.com/item.htm?id=965373226783");
        }
	}
}
