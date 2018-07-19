using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using FlaUI.UIA3;

namespace TypeActivity
{

        [Designer(typeof(TypeActivityDemoDesigner))]
        public sealed class KeyboardType : CodeActivity
        {
            // 定义一个字符串类型的活动输入参数
            public InArgument<string> Selector { get; set; }
            public InArgument<string> Text { get; set; }

            [Browsable(false)]
            public InArgument<string> ImgName { get; set; }

            // 如果活动返回值，则从 CodeActivity<TResult>
            // 并从 Execute 方法返回该值。
            protected override void Execute(CodeActivityContext context)
            {
                // 获取 Text 输入参数的运行时值
                string selector = context.GetValue(this.Selector);
                string text = context.GetValue(this.Text);
                Type(selector, text);

            }

            private void Type(string selector, string text)
            {
                UIA3Automation automation = new UIA3Automation();
                if (automation is null)
                    return;
                try
                {
                    var e1 = automation.GetDesktop();
                    var e2 = e1.FindFirstByXPath(selector);
                    if (selector.ToUpper().Contains("/WINDOW"))
                    {
                        e2.SetForeground();
                        e2.Click();
                        FlaUI.Core.Input.Keyboard.Type(text);
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(this.DisplayName + ":" + ex.ToString());
                    //throw ex;
                }
            }
        }
}
