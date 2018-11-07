using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace vision_form
{
    class TreeLoadXml
    {
        //
        public string[] str_parm = new string[20];
        public string[] str_name = new string[20];
        XmlDocument doc = new XmlDocument();
        StringBuilder sb = new StringBuilder();
        //XML每行的内容
        private string xmlLine = "";
        int num = 0;
        //递归遍历节点内容,最关键的函数
        //加载
        public void Tree_Load(TreeView treeView1, string savepath)
        {
            doc.Load(savepath);
            num = 0;
            RecursionTreeControl(doc.DocumentElement, treeView1.Nodes);//将加载完成的XML文件显示在TreeView控件中
            treeView1.ExpandAll();//展开TreeView控件中的所有项

        }
        public void Xml_Load(string savepath)
        {
            doc.Load(savepath);
            num = 0;
            RecursionXmlControl(doc.DocumentElement);

        }

        public void RecursionXmlControl(XmlNode xmlNode)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)//循环遍历当前元素的子元素集合
            {
                try
                {
                    if (str_name[num]==null)
                    {
                        str_name[num] = node.Attributes["Text"].Value;
                        RecursionXmlControl(node);
                    }
                    else
                    {
                        str_parm[num] = node.Attributes["Parm"].Value;
                        num++;
                    }
                }
                catch (System.Exception ex)
                {
                    //str_parm[num] = node.Attributes["Parm"].Value;
                    //num++;
                }
            }
        }
        /// <summary>
        /// RecursionTreeControl:表示将XML文件的内容显示在TreeView控件中
        /// </summary>
        /// <param name="xmlNode">将要加载的XML文件中的节点元素</param>
        /// <param name="nodes">将要加载的XML文件中的节点集合</param>
        public void RecursionTreeControl(XmlNode xmlNode, TreeNodeCollection nodes)
        {
            
            foreach (XmlNode node in xmlNode.ChildNodes)//循环遍历当前元素的子元素集合
            {
                //XmlNode node1 = node.SelectSingleNode("//bookstore//book[author='" + 变量 + "']");//查找指定节点
                TreeNode new_child = new TreeNode();//定义一个TreeNode节点对象
                //new_child.Name = node.Attributes["Name"].Value;
                
                try
                {
                    new_child.Text = node.Attributes["Text"].Value;
                    nodes.Add(new_child);//向当前TreeNodeCollection集合中添加当前节点
                    RecursionTreeControl(node, new_child.Nodes);//调用本方法进行递归
                }
                catch (System.Exception ex)
                {
                    str_parm[num] = node.Attributes["Parm"].Value;
                    num++;
                }

            }
        }
        public void parseNode(TreeNode tn, StringBuilder sb)
        {
            IEnumerator ie = tn.Nodes.GetEnumerator();
            while (ie.MoveNext())
            {
                TreeNode ctn = (TreeNode)ie.Current;
                xmlLine = GetRSSText(ctn);
                sb.Append(xmlLine);
                //如果还有子节点则继续遍历
                if (ctn.Nodes.Count > 0)
                {
                    parseNode(ctn, sb);
                }
                sb.Append("</Node>");
            }
        }
        //成生RSS节点的XML文本行

        public string GetRSSText(TreeNode node)
        {
            //根据Node属性生成XML文本
            string rssText = "<Node Text=\"" + node.Text + "\" >";
           // string rssText = "<Node Name=\"" + node.Name + "\" Text=\"" + node.Text + "\" >";
            return rssText;
        }
        //保存

        public void SaveXml(TreeView treeView1, string savepath, string[] str_parm = null)
        {
            try
            {
                //写文件头部内容
                //下面是生成RSS的OPML文件
                sb.Clear();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("\r\n");
                sb.Append("<Tree>");
                sb.Append("\r\n");
                int num = 0;
                //遍历根节点
                foreach (TreeNode node in treeView1.Nodes)
                {
                    xmlLine = GetRSSText(node);
                    sb.Append(xmlLine);
                    //递归遍历节点
                    parseNode(node, sb);
                    if(str_parm != null)
                    {
                        sb.Append("\r\n");
                        string rssText = "<Node Parm=\"" + str_parm[num] + "\" >";
                        sb.Append(rssText);
                        sb.Append("</Node>");
                        num++;
                    }
                    
                    sb.Append("</Node>");
                    sb.Append("\r\n");
                }
                sb.Append("</Tree>");
                sb.Append("\r\n");
                //if (File.Exists(savepath))
                //{
                //    File.Delete(savepath);
                //}
                StreamWriter sr = new StreamWriter(savepath, false, System.Text.Encoding.UTF8);
                
                sr.Write(sb.ToString());
                sr.Close();
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                string tem = ex.Message;
                MessageBox.Show("保存失败  " + tem);
            }
        }
    }
}
