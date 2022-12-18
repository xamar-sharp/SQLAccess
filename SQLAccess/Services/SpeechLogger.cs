using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
namespace SQLAccess.Services
{
    public sealed class SpeechLogger : ISpeechLogger
    {
        private static readonly SpeechSynthesizer _synthesizer;
        static SpeechLogger()
        {
            _synthesizer = new SpeechSynthesizer();
        }
        public void SpeakAsync(string text,bool sayDate,bool sayFast)
        {
            PromptBuilder builder = new PromptBuilder();
            if (sayFast)
            {
                builder.StartStyle(new PromptStyle() { Rate = PromptRate.Fast });
            }
            if (sayDate)
            {
                builder.AppendTextWithHint(text, SayAs.Date);
            }
            else
            {
                builder.AppendText(text);
            }
            if (sayFast)
            {
                builder.EndStyle();
            }
            _synthesizer.SpeakAsync(builder);
        }
    }
}
