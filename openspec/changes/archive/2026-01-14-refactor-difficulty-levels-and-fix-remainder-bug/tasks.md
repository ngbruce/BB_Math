# 实现任务清单

## 1. 修改难度枚举定义
- [x] 1.1 修改Core/Difficulty.cs，将枚举值从Easy、Medium、Hard改为LV1、LV2、LV3、LV4、LV5
- [x] 1.2 更新枚举值的注释，反映新的难度级别含义

## 2. 更新难度配置管理器
- [x] 2.1 修改Core/DifficultyConfiguration.cs的GetConfig()方法，实现5个难度级别的配置
- [x] 2.2 LV1配置：个位数加减法（操作数1-9，加减法，减法结果>=0）
- [x] 2.3 LV2配置：100以内加减法（操作数1-99，加减法，减法结果>=0，加法结果<100）
- [x] 2.4 LV3配置：个位数乘除法（操作数1-9，乘除法，除法需保证能整除或有余数）
- [x] 2.5 LV4配置：100以内加减乘除带余数（操作数1-99，五类运算，除法分有无余数）
- [x] 2.6 LV5配置：100以上加减乘除带余数（操作数1-999，五类运算，除法分有无余数）
- [x] 2.7 更新GetAllConfigs()方法，返回5个难度配置
- [x] 2.8 更新GetDifficultyByText()方法，支持新的显示文本

## 3. 修改主窗体逻辑
- [x] 3.1 修改Form1.cs的InitializeDifficultyComboBox()方法，动态初始化cbDifficultySelect控件
- [x] 3.2 清空cbDifficultySelect.Items集合
- [x] 3.3 按顺序添加5个难度选项的显示文本
- [x] 3.4 修改cbDifficultySelect_SelectedIndexChanged()方法，更新难度映射逻辑
- [x] 3.5 更新默认难度为LV1

## 4. 重构出题逻辑
- [x] 4.1 修改Form1.cs的GenNum()方法，根据新难度实现出题逻辑
- [x] 4.2 LV1出题：随机选择加减法，生成两个个位数，减法保证结果>=0
- [x] 4.3 LV2出题：随机选择加减法，生成两个100以内整数，减法保证结果>=0，加法保证结果<100
- [x] 4.4 LV3出题：随机选择乘除法，乘法生成两个个位数，除法反向生成
- [x] 4.5 LV4出题：五类运算同等概率，除法分有无余数两种情况
- [x] 4.6 LV5出题：五类运算同等概率，除法分有无余数两种情况，数值范围扩大
- [x] 4.7 修复currentTypeIndex同步问题，确保随机选择的ExamType能正确同步到lstExamObjects的索引

## 5. 修复余数控件显示bug
- [x] 5.1 在GenNum()方法中，生成题目后同步更新GameStateManager.currentTypeIndex
- [x] 5.2 确保UpdateDisp()方法能根据currentTypeIndex正确显示余数控件
- [x] 5.3 测试LV4和LV5难度下的有余数除法场景

## 6. 更新状态管理器
- [x] 6.1 修改GameStateManager.cs中的currentDifficulty默认值为Difficulty.LV1

## 7. 测试验证
- [x] 7.1 测试5个难度级别下cbDifficultySelect控件显示是否正确
- [x] 7.2 测试每个难度级别下的出题逻辑是否符合规范
- [x] 7.3 测试LV4和LV5下的有余数除法场景，验证余数控件显示
- [x] 7.4 测试难度切换功能是否正常
- [x] 7.5 测试答题验证逻辑（包括有余数除法）

## 8. Bug修复（额外任务）
- [x] 8.1 修复LV3难度包含DivisionWithRemainder的bug
- [x] 8.2 修复CreateDefaultExamObjects()缺少Addition和Subtraction的bug
- [x] 8.3 更新TotalQuestionTypes常量从3到5
- [x] 8.4 更新ExamObject.TotalTypeQty从3到5
- [x] 8.5 修复UpdateDisp()余数控件显示逻辑（使用明确的布尔变量避免条件冲突）
