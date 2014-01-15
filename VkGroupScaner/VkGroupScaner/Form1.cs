using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VkGroupScaner {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private List<string> codes = new List<string>(new[]
        {
            "X93GHO0OYV6OPJ20HDDY",
            "I65GEK95IN3JCUUM3ZK7",
            "P3F02INPWPKC9MUJQ9B5",
            "45KQI6PUXSEO1BGOHYXR",
            "IAMFG1M8P52AOM20UTF4",
            "DILNZHJ9AJXV1QW2S6JQ",
            "U6CDPWP7U2DBRWPPDJTW",
            "DH4S7QT2FIYORXUKLWH7",
            "1QEEKWQQJEMTSU9NEIPK",
            "QOZTZPUI7T4Y3PDQNAPL",
            "F3X23G377AJ5BLYGEHWU",
            "W3RE8XP4TTVCFW6S39M6",
            "BTUDD1SDPVQ451QJ3HT1",
            "QIHECGQX430A2K5B6OVC",
            "HKQLB1ZJZYDYUHT1VQ5N",
            "J37SKTTBZ7HJMGL8NQZ1",
            "FLGKBFG3BZ55OE4LSMBC",
            "EBFBXDQU4ZB2CFV8YJW5",
            "ZRYVH5KFBZT16DDHJYQ4",
            "014K0QADVVRYNYS5H9EX",
            "CEFTR3ADZHMM9O7T4K4F",
            "QLYVLF8GHD45SOVCRN23",
            "5HMPEF92L7WG4QG0LY5J",
            "0UORXR66G9NXRM7G3IE2",
            "5UMIASOO2AQS581XM5Y7",
            "IGYVRMA55I9LNCYYWZFQ",
            "7RXS0PQWOGRF7QTS4UL4",
            "L6FJBCQ77HE61NK4PJCR",
            "OUK3AGU94NOLCDFLGALX",
            "IRLRC3582ETJEFH9QG1T",
            "U6CZXEF173K4Y74SR6Y4",
            "RSD863F6ICH3QG27IVFM",
            "0LEQW58QTEYC6M1AMOAA",
            "JPHJWL46BXGNHKXQRT90",
            "LQENDYC4R0VLAG7RAWDO",
            "85EA6A6B7MOO5RF0DTIJ",
            "O6M31N6PTXA29972T11B",
            "MDA93I5JCNWCTGMTNBY4",
            "IY39URW21HA637TXVI3C",
            "VQE7H3A7EA651O1ENLHI",
            "3F8WXK6DFX9LELHSODJX",
            "NV4EWDVB77WF7JJ5SC4K",
            "SEBSLDL87JT5310ZGA5M",
            "PWDVNDTATO029AVHNR1G",
            "IBKT8SF75HPRPFMC5XHA"
    }
    );

        private IAsyncResult ar;

    private void Form1_Load(object sender, EventArgs e) {
        }

        private int req = 0;
        private void timer1_Tick(object sender, EventArgs e) {
            req++;
            label1.Text = string.Format("total updates: {0}", req);
            HttpWebRequest request =
    (HttpWebRequest)WebRequest.Create("https://api.vk.com/method/wall.get?owner_id=-49315675");
            //"https://api.vk.com/method/users.get?user_id=69841914&v=5.5&access_token=b5962b1e319140539a422829c55835d0448e033bbe55efd5f34e06e5c0bc30eba720318dc3e8d49a9bf71");

            request.Method = "GET";
            request.Accept = "application/json";


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());

            Regex textgetter = new Regex("\"text\":\"(.*?)\"");
            var a = textgetter.Matches(output.ToString());

            textBox1.Text = output.ToString();

            foreach (var VARIABLE in a)
            {
                var s = VARIABLE.ToString();
                var sub = s.Substring(8, s.Length - 8 - 1).Replace("<br>", " ").Split(' ');
                foreach (var code in sub)
                {
                    if (code == string.Empty) { continue; }
                    if (!codes.Contains(code)) {
                        codes.Add(code);
                        textBox3.Text += code + Environment.NewLine;
                        OnNewCode(code);
                    }
                }
            }

            if (NewCode) {
                Action act = Beep;
                if (ar == null || ar.IsCompleted) {
                    ar = act.BeginInvoke(null, null);
                }
            }
            else {
                label2.Text = string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var code in codes) {
                sb.AppendLine(code);
            }
            textBox3.Text = sb.ToString();

            response.Close();
        }

        private void Beep() {
            for (int i = 0; i < 10; i++) {
                Console.Beep();
            }
        }

        private bool NewCode = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (!NewCode) {
                OnNewCode("test button pressed");
            }
            else {
                NewCode = false;
            }
        }

        public void OnNewCode(string s) {
            NewCode = true;
            SetClipbord(s);
            label2.Text = string.Format("Copied: {0}", s);
        }

        public void SetClipbord(String replacementHtmlText)
        {
            Clipboard.SetText(replacementHtmlText, TextDataFormat.Text);
        }

        //b5962b1e319140539a422829c55835d0448e033bbe55efd5f34e06e5c0bc30eba720318dc3e8d49a9bf71
        //69841914
        //https://oauth.vk.com/authorize?client_id=4114156&scope=999999&redirect_uri=http://oauth.vk.com/blank.html&display=page&response_type=token
    }
}
