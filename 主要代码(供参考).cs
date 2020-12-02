using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;   //numerics动态运算库
using OpenXmlPowerTools;
using SolrNet.Utils;
using System.Security.Cryptography;

namespace RSA_1
{
    public partial class Form1 : Form
    {
        public static Form1 form1;




        public Form1()
        {

            
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
                    

            string plain = textBox1.Text;
            byte[] bintext = System.Text.Encoding.UTF8.GetBytes(plain.ToCharArray());
            string bint_str = null;
            foreach(byte b in bintext)
            {
                bint_str += b.ToString();
            }

            //bin_str是明文转换成的string型
            //BigInteger为一个不可变类型，是加入numerics.dll之后可用的大数运算类型

            BigInteger bint_big = BigInteger.Parse(bint_str);   //这个是明文的大整数型-----明文!!!

            BigInteger pub_key = BigInteger.Parse(textBox3.Text.ToString());  //pub_time是加密中幂次方,相当于----公钥!!!

            BigInteger text_common = BigInteger.Parse(textBox5.Text.ToString()); //-----公共模数!!!

            

            //加密的主要步骤

            string ciper_combine = null;
            for(int a=0; a < textBox1.Text.Length; a++)  //对明文进行分组加密，分组长度
            {
                BigInteger text_text1 = BigInteger.Parse(bintext[a].ToString());
                BigInteger ciper_single = mod_exp(text_text1, pub_key, text_common); //单个字符加密得到的密文

                string length_text = ciper_single.ToString();  //单个字符加密得到的密文 转化为 字符串

                if (length_text.Length < textBox5.Text.Length)
                {
                    int differ_length = textBox5.Text.Length - length_text.Length;//定义相差的长度
                    for(int length_time = 0 ; length_time < differ_length ; length_time++)
                    {
                        length_text = "0" + length_text;
                    }
                }
                ciper_combine += length_text;     //对其完成填充
            }
            


            //  bint_big = bint_big % common;


            textBox2.Text = ciper_combine;// (窗体2输出语句)cipertext是一个字符串string，注意！！！
           // MessageBox.Show(bint_big.ToString());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
             
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        public static BigInteger euler;
        public static BigInteger q_big;
        public static BigInteger p_big;
        public static BigInteger common;
        private void button3_Click(object sender, EventArgs e)
        {
            //该按钮是：生成新的公钥密钥

            //第一部分 
            int digits = int.Parse(label8.Text);

            StringBuilder num = GenerateRandNumber(digits);    //运用stringbuilder来构造指定位数的随机数 是 num 与 num_2;
            StringBuilder num_2 = GenerateRandNumber(digits);  //调用已经写好的generateRandNumber来生成指定位数的随机数;

            
            BigInteger num_big = BigInteger.Parse(num.ToString());     //随机的no.1大数类型
            BigInteger num2_big = BigInteger.Parse(num_2.ToString());  //随机的no.2大数类型

            p_big = GeneratePrime(num_big);    //这是80位的 p
            q_big = GeneratePrime(num2_big);   //这是80位的 q

            euler = (p_big - 1) * (q_big-1);   //这是 欧拉函数 φ(n) = (p-1)(q-1)--------重要！！！-------

            //-----1.生成公钥-----
            textBox3.Text = GeneratePublicKey(euler).ToString();

            BigInteger pub_big = BigInteger.Parse(textBox3.Text);

            //-----2.生成私钥-----
            textBox4.Text = inverse(pub_big, euler).ToString();

            //-----3.公共模数-----
            common = p_big * q_big; //公共模数
            textBox5.Text = common.ToString();

            //测试：
            //textBox1.Text = p_big.ToString();
            //textBox2.Text = q_big.ToString();



        }
     


        public static BigInteger exgcd(BigInteger a, BigInteger b, BigInteger[] x, BigInteger[] y)//
        {
            if (b == 0)
            {
                x[0] = 1L;
                y[0] = 0L;
                return a;
            }
            BigInteger d = exgcd(b, a % b, x, y);
            BigInteger tmp = x[0];
            x[0] = y[0];
            y[0] = tmp - a / b * y[0];
            return d;
        }
        
        public BigInteger inverse(BigInteger a, BigInteger m)//扩展欧几里得求乘法逆元 a mod m 的逆元
        {
            BigInteger[] x = { 0 };
            BigInteger[] y = { 0 };
            BigInteger d = exgcd(a, m, x, y);
            if (d == 1)
            {
                return (x[0] % m + m) % m;
            }
            return -1;
        }


        public BigInteger mod_exp(BigInteger a, BigInteger x, BigInteger m)//大数的快速模幂运算-(1)-------a^x(mod m)
        {
            BigInteger res;
            if (x == 0)
                return 1 % m;
            if (x == 1)
                return a % m;
            res = mod_exp(a, x / 2, m);
            res = res * res % m;
            if ((x & 1) == 1)
                res = res * a % m;
            return res;
        }
        public StringBuilder  GenerateRandNumber(int N)  //用于生成一个指定位数(N)的大随机数，80位之类的
        {
                  //生成的随机数的位数--即生成一个N位的随机大整数   
            char[] arrChar = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < N; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num;
        }
        public BigInteger  GeneratePrime(BigInteger source2)//用于生成一个大的素数，调用了miller-rabin检测函数
        {
           
            do
            {
                source2++;
               
            }
            while (IsProbablePrime(source2) == false);
            return source2;
        }
        public BigInteger  GeneratePublicKey(BigInteger source3)//用于生成公钥,source3是往其中传入φ(n),使其在1与φ(n)中生成一个公钥
        {
            int pub_digits;//公钥位数
            pub_digits = trackBar1.Value;
            BigInteger test_pub = BigInteger.Parse(GenerateRandNumber(pub_digits).ToString());//生成一个指定位数的测试值
            do
            {
                test_pub++;

            }

            while(IfCoprime(source3,test_pub)==false); //在1到φ(n)寻找与φ(n)互素的数

            return test_pub;
            
        }
        public BigInteger GeneratePrivateKey(BigInteger pub1, BigInteger euler1) //生成私钥，传入 公钥 和 欧拉函数φ(n)。
        {

            BigInteger test_pri = inverse(pub1, euler1);
            return test_pri;


        }
        public bool IfCoprime(BigInteger n, BigInteger m)//辗转相除法判断两个大数是否互素
        {

            BigInteger t = 0;
            while (m > 0)
            {
                t = n % m;
                n = m;
                m = t;
            }
            if (n == 1) return true;
            else
            return false;
        }


        public bool IsProbablePrime(BigInteger source) //简单通用的miller-rabin素性检测
        {
            int certainty = 2;
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= source - 2);

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }
        public static string Convert_asc2string(BigInteger asciiCode)//ASCLL码 转为 字符串 函数
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }
        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

            //BigInteger t1 = BigInteger.Parse(textBox1.Text);
            //BigInteger t2 = BigInteger.Parse(textBox2.Text);
            //BigInteger t3 = BigInteger.Parse(textBox3.Text);
            //BigInteger tt = inverse(t1, t2);
            //textBox4.Text = bit.ToString();
            //textBox3.Text = tt.ToString();
            //MessageBox.Show("t2:"+textBox2.Text.Length.ToString()+" ,t5:"+textBox5.Text.Length.ToString());
            //string a = "abcdefgh";
            //string b = a.Substring(3,3);
            //textBox1.Text = b;
            
            

      

           


        }

        private void textBox1_TextChanged_1(object sender, EventArgs ei)
        {
            
        }
        public BigInteger Convert_str2big(string string_1)//可使用的自动转换函数,string与bigint之间转换

        {

            BigInteger bigint = BigInteger.Parse(string_1);
            return bigint;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //----------解密开始-----------------------

            int cipher_len = textBox2.Text.Length;   //加密成的密文的长度
            int common_len = textBox5.Text.Length;  //公共模数的长度

            BigInteger cipertext_2 = BigInteger.Parse(textBox2.Text);
            BigInteger  pri_key_2  = BigInteger.Parse(textBox4.Text);
            BigInteger  common_2   = BigInteger.Parse(textBox5.Text);
            /*
            if (textBox2.Text.Length % textBox5.Text.Length != 0)
            {
                MessageBox.Show("警告：您输入的密文不符合解密形式,请确定是否输入有误.");
                Console.ReadLine();     //程序挂起
            }
            */
            
            
                BigInteger times = (cipher_len / common_len);

                string cipher_str = textBox2.Text;
                string plain_combine2 = null;
                string plain_single2 = null;
                //BigInteger plain_big = 0;
                string plain_str = null;
                int head = 0; //定义从最开头开始取，
        
            for(int t1=0; t1<times; t1++)
            {
                
                plain_str = cipher_str.Substring(head, common_len);
                    BigInteger plain_big = BigInteger.Parse(plain_str);

                    plain_big = mod_exp(plain_big, pri_key_2, common_2);
                    
                    plain_single2 = Convert_asc2string(plain_big);

                    plain_combine2 += plain_single2;  //!!!(重要)!!!plain_combine2是组合好的，解密好的明文字符串!!!

                    head += common_len;
            }
            
            textBox1.Text = plain_combine2;

            

           //----------解密结束-----------------------


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
       
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

     

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = null;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text = null;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox3.Text = null;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox4.Text = null;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox5.Text = null;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text = null;
            textBox2.Text = null;
            textBox3.Text = null;
            textBox4.Text = null;
            textBox5.Text = null;
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //----------解密开始-----------------------

            int cipher_len = textBox2.Text.Length;   //加密成的密文的长度
            int common_len = textBox5.Text.Length;  //公共模数的长度

            BigInteger cipertext_2 = BigInteger.Parse(textBox2.Text);
            BigInteger pri_key_2 = BigInteger.Parse(textBox4.Text);
            BigInteger common_2 = BigInteger.Parse(textBox5.Text);
            /*
            if (textBox2.Text.Length % textBox5.Text.Length != 0)
            {
                MessageBox.Show("警告：您输入的密文不符合解密形式,请确定是否输入有误.");
                Console.ReadLine();     //程序挂起
            }
            */


            BigInteger times = (cipher_len / common_len);

            string cipher_str = textBox2.Text;
            string plain_combine2 = null;
            string plain_single2 = null;
            //BigInteger plain_big = 0;
            string plain_str = null;
            int head = 0; //定义从最开头开始取，

            for (int t1 = 0; t1 < times; t1++)
            {

                plain_str = cipher_str.Substring(head, common_len);
                BigInteger plain_big = BigInteger.Parse(plain_str);

                plain_big = mod_exp(plain_big, pri_key_2, common_2);

                plain_single2 = Convert_asc2string(plain_big);

                plain_combine2 += plain_single2;  //!!!(重要)!!!plain_combine2是组合好的，解密好的明文字符串!!!

                head += common_len;
            }

            textBox1.Text = plain_combine2;



            //----------解密结束-----------------------
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)//100bit
            {
                label8.Text = "15";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)//330bit
            {
                label8.Text = "50";
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)//512bit
            {
                label8.Text = "80";
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked == true)//660bit
            {
                label8.Text = "100";
            }
        }

    

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked == true)//1024bit
            {
                label8.Text = "155";//原来155
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = null;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            textBox2.Text = null;
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            textBox3.Text = null;
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            textBox4.Text = null;
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            textBox5.Text = null;
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = null;
            textBox2.Text = null;
            textBox3.Text = null;
            textBox4.Text = null;
            textBox5.Text = null;
        }
     
        private void button11_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            

        }

        private void button12_Click_1(object sender, EventArgs e)//公钥打包
        {
            MessageBox.Show("私钥:\n\r" + textBox4.Text + "\n\r" + "公共模数:" + "\n\r" + textBox5.Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MessageBox.Show("公钥:\n\r" +textBox3.Text + "\n\r" +"公共模数:"+"\n\r"+textBox5.Text);
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {
          
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            label17.Text = trackBar1.Value.ToString();
        }

    
        private void radioButton5_CheckedChanged_2(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true)//800bit
            {
                label8.Text = "122";
            }
        }
    }
}
