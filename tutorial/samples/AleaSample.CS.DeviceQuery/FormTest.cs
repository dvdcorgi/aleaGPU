using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alea;
using Alea.CSharp;
using NUnit.Framework;

namespace DeviceQuery
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var a = Gpu.Default;

            Run();
        }

        private const int Length = 1000000;

        private static void Kernel(int[] result, int[] arg1, int[] arg2)
        {
            var start = blockIdx.x * blockDim.x + threadIdx.x;
            var stride = gridDim.x * blockDim.x;
            for (var i = start; i < result.Length; i += stride)
            {
                result[i] = arg1[i] + arg2[i];
            }
        }

        [GpuManaged, Test]
        public static void Run()
        {
            var gpu = Gpu.Default;

            Console.WriteLine("Dat GPU: " + gpu);

            var lp = new LaunchParam(16, 256);
            var arg1 = Enumerable.Range(0, Length).ToArray();
            var arg2 = Enumerable.Range(0, Length).ToArray();
            var result = new int[Length];

            gpu.Launch(Kernel, lp, result, arg1, arg2);

            var expected = arg1.Zip(arg2, (x, y) => x + y);

            Assert.That(result, Is.EqualTo(expected));

            for (int i = 0; i < result.Count(); i++)
            {
                Console.WriteLine(result.GetValue(i));
            }
        }
    }
}
