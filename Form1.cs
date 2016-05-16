using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace NumberPuzzle
{
    public partial class Form1 : Form
    {
        private int length = 3;
        private int lengthBefore = 3;
        private int[,] numMat = new int[3,3];
        private int[,] numMatBefore = new int[3, 3];
        private Label[,] numLabel = new Label[3, 3];
        private Point[,] numPoint = new Point[3, 3];
        private bool[,] sorted = new bool[3, 3];
        private int specialI = 2;
        private int specialJ = 2;
        private int specialIT = 2;
        private int specialJT = 2;
        private Color color = new Color();
        private Color scolor = new Color();
        private Font font = null;
        private ArrayList stateNodeLinkD = new ArrayList();

        public Form1()
        {
            InitializeComponent();
            color = Color.Transparent;
            scolor = Color.DarkRed;
            font=new Font("宋体", 110 / 3, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = Color.LightPink;
            this.comboBox1.SelectedIndex = 1;
            initNumMat();
            initPanelMain();
        }

        private void initPanelMain()
        {
            this.panelMain.Controls.Clear();
            int w = (panelMain.Width - 10) / length;
            int h = panelMain.Height / length;

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    numPoint[i, j] = new Point(j * panelMain.Height / length, i * panelMain.Height / length);
                    numLabel[i, j] = new Label();
                    numLabel[i, j].BackColor = color;
                    numLabel[i, j].Text = numMat[i, j].ToString();
                    numLabel[i, j].Size = new Size(w, h);
                    numLabel[i, j].Location = new Point(numPoint[i, j].X, numPoint[i, j].Y);
                    numLabel[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    numLabel[i, j].Font = font;
                    numLabel[i, j].BorderStyle = BorderStyle.Fixed3D;

                    if (numMat[i,j] == length * length)
                    {
                        numLabel[i, j].Text = "";
                        numLabel[i, j].BackColor = scolor;
                        specialI = i;
                        specialJ = j;
                    }
                    this.panelMain.Controls.Add(numLabel[i, j]);
                }
            }
        }

        private void initNumMat()
        {
            int n = 1;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    numMat[i, j] = n;
                    n++;
                }
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            this.LabelHour.Text = "00";
            this.labelMinute.Text = "00";
            this.labelSecond.Text = "00";
            this.labelSteps1.Text = "0";
            this.labelRestSteps.Text = "0";
            this.LabelStepsDone1.Text = "0";
            this.timer1.Enabled = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (panelMain.Controls.Count != 0)
            {
                panelMain.Controls.RemoveAt(panelMain.Controls.Count - 1);
            }
            else
            {
                timer1.Stop();
                lengthBefore = length;
                while (true)
                {
                    randomNumMat();
                    if (canSolved(numMat))
                    {
                        break;
                    }
                }
                initPanelMain(); 
                Execute();
            }
            
        }

        private void randomNumMat()
        {
            Random ra = new Random();
           
            numMat = new int[length, length];
            sorted = new bool[length, length];
            numMatBefore = new int[length, length];
            initNumMat();
            for (int i = 0; i < length * length; i++)
            {
                int x1 = ra.Next(0, length);
                int y1 = ra.Next(0, length);
                int x2 = ra.Next(0, length);
                int y2 = ra.Next(0, length);
                int temp = numMat[x1, y1];
                numMat[x1, y1] = numMat[x2, y2];
                numMat[x2, y2] = temp;
            }
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    numMatBefore[i, j] = numMat[i, j];
                    if (numMat[i, j] == length * length)
                    {
                        specialJ = j;
                        specialI = i;
                    }
                }
            }
            specialIT = specialI;
            specialJT = specialJ;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lengthBefore = length;
            length = this.comboBox1.SelectedIndex + 2;      
           
            font = new Font("宋体", 110 / length, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            numMat = new int[length, length];
            sorted = new bool[length, length];
            numLabel = new Label[length, length];
            numPoint = new Point[length, length];
            initNumMat();
            initPanelMain();
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            this.panelMain.Controls.Clear();
            length = lengthBefore;
            this.comboBox1.SelectedIndex = lengthBefore - 2;
            numMat = numMatBefore;
            numLabel = new Label[length, length];
            numPoint = new Point[length, length];
            sorted=new bool[length,length];
            for (int i = 0; i < length; i++)
            {
                for(int j=0;j<length;j++)
                {
                    if(numMat[i,j]==length*length)
                    {
                        specialI=i;
                        specialJ=j;
                    }
                }
            }
            this.LabelStepsDone1.Text = "0";
            this.labelRestSteps.Text = this.labelSteps1.Text;
            specialIT = specialI;
            specialJT = specialJ;
            initPanelMain();
            Execute();
        }

        private void buttonBegin_Click(object sender, EventArgs e)     
        {
            this.LabelHour.Text = "00";
            this.labelMinute.Text = "00";
            this.labelSecond.Text = "00";
            if (!canSolved(numMat))
            {
                MessageBox.Show("Can not be solved !Please generate Number Matrix randomly again.", "Tips");
                return;
            }
            this.labelSteps1.Text = stateNodeLinkD.Count.ToString();
            this.labelRestSteps.Text = stateNodeLinkD.Count.ToString();
            this.LabelStepsDone1.Text = "0";
            this.timer2.Enabled = true;
            this.buttonRestart.Enabled = false;
            this.comboBox1.Enabled = false;
            this.buttonCreate.Enabled = false;
            timer2.Start();
            this.timer3.Enabled = true;
            this.timer3.Start();
        }

        private void Execute()
        {
            stateNodeLinkD.Clear();
            if (!canSolved(numMat))
            {
                return;
            }
           for (int i = 0; i < length - 1; i++)
           {
                for (int y = i; y < length; y++)
                {
                    sortMat(i * length + y);
                    sorted[i, y] = true;
                }
                for (int x = i + 1; x < length; x++)
                {
                    sortMat(x * length + i);
                    sorted[x, i] = true;
                }
            }        
        }

        private void sortMat(int position)
        {
            int i = position / length;
            int j = position % length;
            if(numMat[i,j]==position+1)
            {
                return;
            }
            int si = 0, sj = 0;
            for(int ci=0;ci<length;ci++)
            {
                for(int cj=0;cj<length;cj++)
                {
                    if(numMat[ci,cj]==position+1)
                    {
                        si = ci;
                        sj = cj;
                        break;
                    }
                }
            }
            if(sj==length-1&&position%length==length-1&&(specialJ!=position%length||specialI!=position/length)&&position%length-sj==-1)
            {
                sortSpecialCol(position);
                return;
            }
            if(si==length-1&&position/length==length-1&&(specialI!=position/length||specialJ!=position%length)&&position/length-si==-1)
            {
                sortSpecialRow(position);
                return;
            }
            sortNormalNum(si,sj,position);
        }

        public void moveSepcialNum(int dstI,int dstJ ,int si,int sj,int flag)
        {
            if (flag == -1||flag==1)
            {
                if (dstJ < specialJ)
                {
                    swapNumPos(3);
                }
                if (dstJ > specialJ)
                {
                    swapNumPos(4);
                }
                if (specialJ == dstJ)
                {
                    if (specialI > dstI)
                    {
                        swapNumPos(1);
                    }
                    if (specialI < dstI)
                    {
                        swapNumPos(2);
                    }
                    if (specialI == dstI)
                    {
                        return;
                    }
                }
                moveSepcialNum(dstI, dstJ, si,sj,flag);
            } 
            if (flag == 0)
            {
                if (specialJ < dstJ)
                {
                    if (specialI == si && specialJ == sj - 1)
                    {
                        if (specialI != length - 1)
                        {
                            swapNumPos(2);
                        }
                        else
                        {
                            swapNumPos(1);
                        }
                    }
                    else
                    {
                        swapNumPos(4);
                    }
                }
                if (specialJ > dstJ)
                {
                    if (specialI == si && specialJ == sj + 1)
                    {
                        if (specialI != length - 1)
                        {
                            swapNumPos(2);
                        }
                        else
                        {
                            swapNumPos(1);
                        }
                    }
                    else
                    {
                        if (sorted[specialI, specialJ - 1] == true)
                        {
                            if (sj == specialJ && si == specialI + 1)
                            {
                                if (specialJ != length - 1)
                                {
                                    swapNumPos(4);
                                }
                            }
                            if (specialI != length - 1)
                            {
                                swapNumPos(2);
                            }
                        }
                        else
                        {
                            swapNumPos(3);
                        }
                    }
                }
                if (specialJ == dstJ)
                {
                    if (specialJ != sj)
                    {
                        if (specialI > dstI)
                        {
                            swapNumPos(1);
                        }
                        if (specialI < dstI)
                        {
                            swapNumPos(2);
                        }
                        if (specialI == dstI)
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (specialI > dstI)
                        {
                            if (specialI - 1 == si)
                            {
                                if (specialJ != length - 1)
                                {
                                    swapNumPos(4);
                                    swapNumPos(1);
                                    swapNumPos(1);
                                    swapNumPos(3);
                                }
                                else
                                {
                                    swapNumPos(3);
                                    swapNumPos(1);
                                    swapNumPos(1);
                                    swapNumPos(4);
                                }
                            }
                            else
                            {
                                swapNumPos(1);
                            }

                        }
                        if (specialI < dstI)
                        {
                            if (specialI + 1 == si)
                            {
                                if (specialI != length - 1)
                                {
                                    swapNumPos(4);
                                    swapNumPos(2);
                                    swapNumPos(2);
                                    swapNumPos(3);
                                }
                                else
                                {
                                    swapNumPos(3);
                                    swapNumPos(2);
                                    swapNumPos(2);
                                    swapNumPos(4);
                                }
                            }
                            else
                            {
                                swapNumPos(2);
                            }
                        }
                        if (specialI == dstI)
                        {
                            return;
                        }
                    }        
                }
                moveSepcialNum(dstI, dstJ, si,sj, flag);
            }
        }

        private void sortSpecialCol(int position)
        {
            int i = position / length;
            moveSepcialNum(i + 1, length - 3, position / length+1, position %length,-1);
            int[] moveDirection = {1,4,4,2,3,1,3,2,4 };
            for(int x=0;x<moveDirection.Length;x++)
            {
                swapNumPos(moveDirection[x]);
            }
        }

        private void sortSpecialRow(int position)
        {
            int j = position % length;
            moveSepcialNum(length - 1, j + 2,position/length,position%length+1,1);
            int[] moveDirection = { 3, 3, 1, 4,2,4,  1, 3,3, 2, 4 };
            for (int x = 0; x < moveDirection.Length; x++)
            {
                swapNumPos(moveDirection[x]);
            }
        }

        private void sortNormalNum(int si,int sj,int position)
        {
            if(si>=position/length&&sj>position%length)
            {
                sortCondition1(si, sj, position / length, position % length);
                return;
            }
            if(si>position/length&&sj<=position%length)
            {
                sortCondition2(si, sj, position / length, position % length);
                return;
            }
            if(si<position/length&&sj>position%length)
            {
                sortCondition3(si, sj, position / length, position % length);
                return;
            }
        }

        private void sortCondition1(int si,int sj,int di,int dj)
        {
            if (si == length - 1 && di == length - 1 && (specialI != di || specialJ != dj) && dj - sj == -1)
            {
                sortSpecialRow(di * length + dj);
                return;
            }
            moveSepcialNum(si, sj - 1,si,sj, 0);
            while(sj!=dj)
            {
                if (si == length - 1 && di == length - 1 && specialI != di && dj - sj == -1)
                {
                    sortSpecialRow(di*length+dj);
                    return;
                }
                swapNumPos(4);
                sj--;
                if(sj!=dj)
                {
                    if (si == length - 1 && di == length - 1 && (specialI != di || specialJ != dj) && dj - sj == -1)                
                    {
                        sortSpecialRow(di * length + dj);
                        return;
                    }
                    if(specialI!=length-1)
                    {
                        int[] moveDirection = { 2, 3, 3, 1 };
                        for (int x = 0; x < moveDirection.Length; x++)
                        {
                            swapNumPos(moveDirection[x]);
                        }
                    }
                    else
                    {
                        int[] moveDirection = { 1, 3, 3, 2 };
                        for (int x = 0; x < moveDirection.Length; x++)
                        {
                            swapNumPos(moveDirection[x]);
                        }
                    }
                }
            }
            if(si!=di)
            {
                int[] moveDirection = { 1, 3 };
                for (int x = 0; x < moveDirection.Length; x++)
                {
                    swapNumPos(moveDirection[x]);
                }
            }
            while(si!=di)
            {
                swapNumPos(2);
                si--;
                if (si != di)
                {
                    int[] moveDirection = { 4, 1, 1, 3 };
                    for (int x = 0; x < moveDirection.Length; x++)
                    {
                        swapNumPos(moveDirection[x]);
                    }
                }
            }
        }

        private void sortCondition2(int si, int sj, int di, int dj)
        {
            if(sj==dj)
            {
                if (sj == length - 1 && dj == length - 1 && (specialJ != dj || specialI != di) && di - si == -1)
                {
                    sortSpecialCol(di * length + dj);
                    return;
                }
                moveSepcialNum(si - 1, sj,si,sj, 0);
            }
            else
            {
                moveSepcialNum(si, sj + 1,si,sj,0);
                while(sj!=dj)
                {
                    swapNumPos(3);
                    sj++;
                    if(sj!=dj)
                    {
                        if (sj == length - 1 && dj == length - 1 && (specialJ != dj || specialI != di) && di - si == -1) 
                        {
                            sortSpecialCol(di * length + dj);
                            return;
                        }
                        if(specialI!=length-1)
                        {
                            int[] moveDirection = { 2, 4, 4, 1 };
                            for (int x = 0; x < moveDirection.Length; x++)
                            {
                                swapNumPos(moveDirection[x]);
                            }
                        }
                        else
                        {
                            int[] moveDirection = { 1, 4, 4, 2 };
                            for (int x = 0; x < moveDirection.Length; x++)
                            {
                                swapNumPos(moveDirection[x]);
                            }
                        }
                       
                    }               
                }
                if (sj == length - 1 && dj == length - 1 && (specialJ != dj || specialI != di) && di - si == -1)
                {
                    sortSpecialCol(di * length + dj);
                    return;
                }
                if(specialI==di+1)
                {
                    int[] moveDirection = { 2, 4, 4,1,1, 3 };
                    for (int x = 0; x < moveDirection.Length; x++)
                    {
                        swapNumPos(moveDirection[x]);
                    }
                }
                else
                {
                    int[] moveDirection = {  1, 4 };
                    for (int x = 0; x < moveDirection.Length; x++)
                    {
                        swapNumPos(moveDirection[x]);
                    }
                }
                
            }
            while(si!=di)
            {
                if (sj == length - 1 && dj == length - 1 && (specialJ != dj || specialI != di) && di - si == -1)
                {
                    sortSpecialCol(di * length + dj);
                    return;
                }
                swapNumPos(2);
                si--;
                if (si != di)
                {              
                    if (sj==length-1 && dj == length - 1 && (specialJ != dj||specialI!=di) && di - si == -1)
                    {
                        sortSpecialCol(di * length + dj);
                        return;
                    }
                    if(specialJ!=length-1)
                    {
                        int[] moveDirection = { 4, 1, 1, 3 };
                        for (int x = 0; x < moveDirection.Length; x++)
                        {
                            swapNumPos(moveDirection[x]);
                        }
                    }
                    else
                    {
                        int[] moveDirection = { 3, 1, 1, 4 };
                        for (int x = 0; x < moveDirection.Length; x++)
                        {
                            swapNumPos(moveDirection[x]);
                        }
                    }            
                }
            }
        }

        private void sortCondition3(int si, int sj, int di, int dj)
        {
            if(si==di)
            {
                moveSepcialNum(si, sj - 1,si,sj, 0);
                while(sj!=dj)
                {
                    if (si == length - 1 && di == length - 1 && (specialI != di || specialJ != dj) && dj - sj == -1)
                    {
                        sortSpecialRow(di * length + dj);
                        return;
                    }
                    swapNumPos(4);
                    sj--;
                    if(sj!=dj)
                    {
                        if (si == length - 1 && di == length - 1 && (specialI != di || specialJ != dj) && dj - sj == -1)
                        {
                            sortSpecialRow(di * length + dj);
                            return;
                        }
                        int[] moveDirection = { 1, 3, 3, 2 };
                        for (int x = 0; x < moveDirection.Length; x++)
                        {   
                            swapNumPos(moveDirection[x]);
                        }
                    }

                }
            }
            else
            {
                moveSepcialNum(si + 1, sj,si,sj, 0);              
                while(si!=di)
                {
                    swapNumPos(1);
                    si++;
                    if(si!=di)
                    {
                        if (sj == length - 1)
                        {
                            int[] moveDirection = { 3, 2, 2, 4 };
                            for (int x = 0; x < moveDirection.Length; x++)
                            {
                                swapNumPos(moveDirection[x]);
                            }
                        }
                        else
                        {
                            int[] moveDirection = { 4, 2, 2, 3 };
                            for (int x = 0; x < moveDirection.Length; x++)
                            {
                                swapNumPos(moveDirection[x]);
                            }
                        }
                        
                    }
                }
                if (si == length - 1 && di == length - 1 && (specialI != di || specialJ != dj) && dj - sj == -1)
                {
                    sortSpecialRow(di * length + dj);
                    return;
                }
                if(specialJ==dj+1)
                {
                    int[] moveDirection = { 4, 2,2, 3, 3,1 };
                    for (int x = 0; x < moveDirection.Length; x++)
                    {
                        swapNumPos(moveDirection[x]);
                    }
                }
                else
                {
                    int[] moveDirection = {  3, 2 };
                    for (int x = 0; x < moveDirection.Length; x++)
                    {
                        swapNumPos(moveDirection[x]);
                    }
                }
                while(sj!=dj)
                {
                    if (si == length - 1 && di == length - 1 && (specialI != di || specialJ != dj) && dj - sj == -1)
                    {
                        sortSpecialRow(di * length + dj);
                        return;
                    }
                    swapNumPos(4);
                    sj--;
                    if(sj!=dj)
                    {
                        if (si == length - 1 && di == length - 1 && (specialI != di || specialJ != dj) && dj - sj == -1)
                        {
                            sortSpecialRow(di * length + dj);
                            return;
                        }
                        if (di == length-1)
                        {
                            int[] moveDirection = { 1, 3, 3, 2 };
                            for (int x = 0; x < moveDirection.Length; x++)
                            {
                                swapNumPos(moveDirection[x]);
                            }
                        }
                        else
                        {
                            int[] moveDirection = { 2, 3,3,1 };
                            for (int x = 0; x < moveDirection.Length; x++)
                            {
                                swapNumPos(moveDirection[x]);
                            }
                        }
                    }
                }

            }
        }

        private void swapNumPos(int flag)
        {
            try
            {
                if (flag == 1)
                {
                    stateNodeLinkD.Add(flag);
                    int[,] numMattemp = new int[length, length];
                    Array.Copy(numMat, numMattemp, length * length);
                    int num = numMattemp[specialI, specialJ];
                    numMattemp[specialI, specialJ] = numMattemp[specialI - 1, specialJ];
                    numMattemp[specialI - 1, specialJ] = num;
                    numMat = numMattemp;
                    specialI -= 1;
                }
                if (flag == 2)
                {
                    stateNodeLinkD.Add(flag);
                    int[,] numMattemp = new int[length, length];
                    Array.Copy(numMat, numMattemp, length * length);
                    int num = numMattemp[specialI, specialJ];
                    numMattemp[specialI, specialJ] = numMattemp[specialI + 1, specialJ];
                    numMattemp[specialI + 1, specialJ] = num;
                    numMat = numMattemp;
                    specialI += 1;
                }
                if (flag == 3)
                {
                    stateNodeLinkD.Add(flag);
                    int[,] numMattemp = new int[length, length];
                    Array.Copy(numMat, numMattemp, length * length);
                    int num = numMattemp[specialI, specialJ];
                    numMattemp[specialI, specialJ] = numMattemp[specialI, specialJ - 1];
                    numMattemp[specialI, specialJ - 1] = num;
                    numMat = numMattemp;
                    specialJ -= 1;
                }
                if (flag == 4)
                {
                    stateNodeLinkD.Add(flag);
                    int[,] numMattemp = new int[length, length];
                    Array.Copy(numMat, numMattemp, length * length);
                    int num = numMattemp[specialI, specialJ];
                    numMattemp[specialI, specialJ] = numMattemp[specialI, specialJ + 1];
                    numMattemp[specialI, specialJ + 1] = num;
                    numMat = numMattemp;
                    specialJ += 1;
                }
            }
           catch
            {
                return;
           }
        }

        private void swapLabels(int flag)
        { 
            try
            {
                if (flag == 1)
                {
                    numLabel[specialIT, specialJT].Text = numLabel[specialIT - 1, specialJT].Text;
                    numLabel[specialIT, specialJT].BackColor = color;
                    numLabel[specialIT - 1, specialJT].Text = "";
                    numLabel[specialIT - 1, specialJT].BackColor = scolor;
                    specialIT = specialIT - 1;
                    return;
                }
                if (flag == 2)
                {
                    numLabel[specialIT, specialJT].Text = numLabel[specialIT + 1, specialJT].Text;
                    numLabel[specialIT, specialJT].BackColor = color;
                    numLabel[specialIT + 1, specialJT].Text = "";
                    numLabel[specialIT + 1, specialJT].BackColor = scolor;
                    specialIT = specialIT + 1;
                    return;
                }
                if (flag == 3)
                {
                    numLabel[specialIT, specialJT].Text = numLabel[specialIT, specialJT - 1].Text;
                    numLabel[specialIT, specialJT].BackColor = color;
                    numLabel[specialIT, specialJT - 1].Text = "";
                    numLabel[specialIT, specialJT - 1].BackColor = scolor;
                    specialJT = specialJT - 1;
                    return;
                }
                if (flag == 4)
                {
                    numLabel[specialIT, specialJT].Text = numLabel[specialIT, specialJT + 1].Text;
                    numLabel[specialIT, specialJT].BackColor = color;
                    numLabel[specialIT, specialJT + 1].Text = "";
                    numLabel[specialIT, specialJT + 1].BackColor = scolor;
                    specialJT = specialJT + 1;
                }
            }
            catch
            {
                return;
            }          
        }

        private int calInverseNum(int[,] status)
        {
            int reverse = 0;
            for(int i=0;i<length*length;i++)
            {
                for(int j=i+1;j<length*length;j++)
                {
                    int k = i / length;
                    int m = i% length;
                    if(status[k,m]>status[j/length,j%length])
                    {
                        reverse++;
                    }
                }
            }
            return reverse;
        }

        private bool canSolved(int[,] status)
        {
            if((calInverseNum(status)+specialI+specialJ)%2==0)
            {
                return true;
            }
            return false;
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            this.timer2.Stop();
            this.timer3.Stop();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fontsToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (this.fontDialog1.ShowDialog() == DialogResult.OK)
            {
                font = fontDialog1.Font;
                foreach (Label b in this.panelMain.Controls)
                {
                    b.Font = fontDialog1.Font;
                }

            }
        }

        private void numberColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                color = colorDialog1.Color;
                foreach (Label b in this.panelMain.Controls)
                {
                    if (b.Text != "")
                    {
                        b.BackColor = colorDialog1.Color;
                    }
                }
            }
        }

        private void specialColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                scolor = colorDialog1.Color;
                foreach (Label b in this.panelMain.Controls)
                {
                    if (b.Text == "")
                    {
                        b.BackColor = colorDialog1.Color;
                        return;
                    }
                }
            }
        }

        private void buttonColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.buttonBegin.BackColor = colorDialog1.Color;
                this.buttonCreate.BackColor = colorDialog1.Color;
                this.buttonPause.BackColor = colorDialog1.Color;
                this.buttonRestart.BackColor = colorDialog1.Color;
                this.buttonNextStep.BackColor = colorDialog1.Color;
                this.buttonContinue.BackColor = colorDialog1.Color;
            }
        }

        private void colorToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.ForeColor = colorDialog1.Color;
            }
        }

        private void commandPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.commandPanelToolStripMenuItem.Checked == true)
            {
                this.panelSide.Visible = true;
            }
            else
            {
                this.panelSide.Visible = false;
            }    
        }

        private void defaultToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            color = Color.Transparent;
            scolor = Color.DarkRed;
            font = new Font("宋体", 110 / length, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            foreach (Label b in this.panelMain.Controls)
            {
                b.BackColor = Color.Transparent;
                b.Font = new Font("宋体", 110 / length, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                if (b.Text == "")
                {
                    b.BackColor = Color.DarkRed;
                }
            }
        }

        private void backgroundToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "图片（*.jpg/*.png/*.gif/*.bmp）|*.jpg;*.png;*.gif;*.bmp";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var filename = dialog.FileName;
                try
                {
                    Bitmap bitmap = new Bitmap(filename);
                    this.BackgroundImage = bitmap;
                }
                catch
                {
                    return;
                }
            }        
        }

        private void defaultToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.panelSide.Visible = true;
            this.commandPanelToolStripMenuItem.Checked = true;
            this.ForeColor = Color.LightPink;
        }

        private void sterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.sterToolStripMenuItem.Checked = true;
            this.centerToolStripMenuItem.Checked = false;
            this.zomeToolStripMenuItem.Checked = false;
            this.noneToolStripMenuItem.Checked = false;
            this.tileToolStripMenuItem.Checked = false;
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackgroundImageLayout = ImageLayout.Center;
            this.sterToolStripMenuItem.Checked = false;
            this.centerToolStripMenuItem.Checked = true;
            this.zomeToolStripMenuItem.Checked = false;
            this.noneToolStripMenuItem.Checked = false;
            this.tileToolStripMenuItem.Checked = false;
        }

        private void zomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.sterToolStripMenuItem.Checked = false;
            this.centerToolStripMenuItem.Checked = false;
            this.zomeToolStripMenuItem.Checked = true;
            this.noneToolStripMenuItem.Checked = false;
            this.tileToolStripMenuItem.Checked = false;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackgroundImageLayout = ImageLayout.Tile;
            this.sterToolStripMenuItem.Checked = false;
            this.centerToolStripMenuItem.Checked = false;
            this.zomeToolStripMenuItem.Checked = false;
            this.noneToolStripMenuItem.Checked = false;
            this.tileToolStripMenuItem.Checked = true;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackgroundImageLayout = ImageLayout.None;
            this.sterToolStripMenuItem.Checked = false;
            this.centerToolStripMenuItem.Checked = false;
            this.zomeToolStripMenuItem.Checked = false;
            this.noneToolStripMenuItem.Checked = true;
            this.tileToolStripMenuItem.Checked = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {      
            if(stateNodeLinkD.Count==0)
            {
                this.label3.Visible = true;
                this.comboBox1.Enabled = true;
                this.buttonCreate.Enabled = true;
                this.buttonRestart.Enabled = true;
                timer3.Stop();
                timer2.Stop();                       
                return;
            }
           if(stateNodeLinkD.Count!=0)
           {
               int flag = (int)stateNodeLinkD[0];
               swapLabels(flag);
               stateNodeLinkD.RemoveAt(0);
               this.LabelStepsDone1.Text = (Convert.ToInt32(this.LabelStepsDone1.Text) + 1).ToString();      
           }
           this.labelRestSteps.Text = this.stateNodeLinkD.Count.ToString();               
        }

        private void sceneryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\Resource\\1.jpg";
                Bitmap bitmap = new Bitmap(path);
                this.BackgroundImage = bitmap;
                scolor = Color.Pink;
                Color bcolor = Color.FromArgb(255,128,128);
                this.buttonBegin.BackColor = bcolor;
                this.buttonCreate.BackColor = bcolor;
                this.buttonPause.BackColor = bcolor;
                this.buttonRestart.BackColor = bcolor;
                this.buttonNextStep.BackColor = bcolor;
                this.buttonContinue.BackColor = bcolor;
                foreach (Label b in this.panelMain.Controls)
                {
                    if (b.Text == "")
                    {
                        b.BackColor = Color.Pink;
                        break;
                    }
                }
                this.ForeColor = Color.DarkRed;
            }
            catch
            {
                return;
            }  
        }

        private void manToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\Resource\\2.jpg";
                Bitmap bitmap = new Bitmap(path);
                this.BackgroundImage = bitmap;
                scolor = Color.White;
                this.buttonBegin.BackColor = Color.White;
                this.buttonCreate.BackColor = Color.White;
                this.buttonPause.BackColor = Color.White;
                this.buttonRestart.BackColor = Color.White;
                this.buttonNextStep.BackColor = Color.White;
                this.buttonContinue.BackColor = Color.White;
                foreach (Label b in this.panelMain.Controls)
                {
                    if (b.Text == "")
                    {
                        b.BackColor = Color.White;
                        break;
                    }
                }
                this.ForeColor = Color.Pink;
            }
            catch
            {
                return;
            }
        }

        private void animalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\Resource\\4.jpg";
                Bitmap bitmap = new Bitmap(path);
                this.BackgroundImage = bitmap;
                scolor = Color.LightGreen;
                this.buttonBegin.BackColor = Color.DarkGreen;
                this.buttonCreate.BackColor = Color.DarkGreen;
                this.buttonPause.BackColor = Color.DarkGreen;
                this.buttonRestart.BackColor = Color.DarkGreen;
                this.buttonNextStep.BackColor = Color.DarkGreen;
                this.buttonContinue.BackColor = Color.DarkGreen;
                foreach (Label b in this.panelMain.Controls)
                {
                    if (b.Text == "")
                    {
                        b.BackColor = Color.LightGreen;
                        break;
                    }
                }
                this.ForeColor = Color.LightSkyBlue;
            }
           catch
            {
                return;
           }
        }

        private void cartoonToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\Resource\\3.jpg";
                Bitmap bitmap = new Bitmap(path);
                this.BackgroundImage = bitmap;
                scolor = Color.DarkRed;
                this.buttonBegin.BackColor = Color.DarkRed;
                this.buttonCreate.BackColor = Color.DarkRed;
                this.buttonPause.BackColor = Color.DarkRed;
                this.buttonRestart.BackColor = Color.DarkRed;
                this.buttonNextStep.BackColor = Color.DarkRed;
                this.buttonContinue.BackColor = Color.DarkRed;
                foreach (Label b in this.panelMain.Controls)
                {
                    if (b.Text == "")
                    {
                        b.BackColor = Color.DarkRed;
                        break;
                    }
                }
                this.ForeColor = Color.LightPink;
            }
           catch
           {
                return;
           }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if(this.label3.Visible==true)
            {
                this.label3.Visible = false;
            }
            else
            {
                this.label3.Visible = true ;
            }
            int hour = Convert.ToInt32(this.LabelHour.Text);
            int minute = Convert.ToInt32(this.labelMinute.Text);
            int second = Convert.ToInt32(this.labelSecond.Text);
            if(second==59)
            {
                second = 0;
                minute += 1;
            }
            else
            {
                second++;
            }
            if(minute==60)
            {
                minute = 0;
                hour += 1;
            }
            if(second>9)
            {
                this.labelSecond.Text = second.ToString();
            }
            else
            {
                this.labelSecond.Text ="0"+second.ToString();
            }
            if (minute > 9)
            {
                this.labelMinute.Text = minute.ToString();
            }
            else
            {
                this.labelMinute.Text = "0" + minute.ToString();
            }
            if (hour > 9)
            {
                this.LabelHour.Text = hour.ToString();
            }
            else
            {
                this.LabelHour.Text = "0" + hour.ToString();
            }
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            this.timer3.Start();
            this.timer2.Start();
        }

        private void buttonNextStep_Click(object sender, EventArgs e)
        {
            if (stateNodeLinkD.Count == 0)
            {
                this.label3.Visible = true;
                this.comboBox1.Enabled = true;
                this.buttonCreate.Enabled = true;
                this.buttonRestart.Enabled = true;
                timer3.Stop();
                timer2.Stop();
                return;
            }
            timer3.Stop();
            timer2.Stop();
            
            int flag = (int)stateNodeLinkD[0];
            swapLabels(flag);
            stateNodeLinkD.RemoveAt(0);
            this.LabelStepsDone1.Text = (Convert.ToInt32(this.LabelStepsDone1.Text) + 1).ToString();
            this.labelRestSteps.Text = this.stateNodeLinkD.Count.ToString();
            this.labelSteps1.Text = (Convert.ToInt32(LabelStepsDone1.Text) + Convert.ToInt32(labelRestSteps.Text)).ToString() ;
        }

        private void numericUpDownFrequency_ValueChanged(object sender, EventArgs e)
        {
            this.timer2.Interval = Convert.ToInt32(this.numericUpDownFrequency.Value);
        }
    }
}
