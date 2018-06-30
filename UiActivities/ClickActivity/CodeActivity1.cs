using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Activities.Presentation.Metadata;
using FlaUI.UIA3;

namespace ClickActivity
{
    public enum ClickTypeEnum { SingleLeft, DoubleLeft, SingleRight, DoubleRight }

    [Designer(typeof(ClickActivityDemoDesigner))]
    public sealed class MouseClick : CodeActivity
    {
        // 定义一个字符串类型的活动输入参数
        public InArgument<string> Text { get; set; }

        
        public ClickTypeEnum  MouseButton { get; set; }

        // 如果活动返回值，则从 CodeActivity<TResult>
        // 并从 Execute 方法返回该值。
        protected override void Execute(CodeActivityContext context)
        {
            // 获取 Text 输入参数的运行时值
            string text = context.GetValue(this.Text);
            //ClickTypeEnum button = MouseButton);
            Click(text, MouseButton);

        }

        private void Click(string xpath, ClickTypeEnum button)
        {
            UIA3Automation automation = new UIA3Automation();
            if (automation is null)
                return;
            try
            {
                var e1 = automation.GetDesktop();
                var e2 = e1.FindFirstByXPath(xpath);
                if (xpath.ToUpper().Contains("/WINDOW"))
                    e2.SetForeground();
                switch (button)
                {
                    case ClickTypeEnum.SingleLeft:
                        e2.Click();
                        break;
                    case ClickTypeEnum.SingleRight:
                        e2.RightClick();
                        break;
                    case ClickTypeEnum.DoubleLeft:
                        e2.DoubleClick();
                        break;
                    case ClickTypeEnum.DoubleRight:
                        e2.RightDoubleClick();
                        break;
                }

            }
            catch (Exception ex)
            {
                
                Console.WriteLine(this.DisplayName + ":"+ex.ToString());
                throw ex;
            }
        }
    }
}
