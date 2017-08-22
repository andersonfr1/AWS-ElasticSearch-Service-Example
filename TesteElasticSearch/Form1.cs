using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesteElasticSearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var listDocuments = new AWSElasticSearchServiceBO().GetFullTextSearch(txtSearch.Text, txtClienteId.Text);
            dtIscas.DataSource = listDocuments.Documents;
        }

        //
        private void button1_Click(object sender, EventArgs e)
        {
            new AWSElasticSearchServiceBO().BulkInsertDocuments();
            MessageBox.Show("Finished!");
        }

        //
        private void button3_Click(object sender, EventArgs e)
        {
            new AWSElasticSearchServiceBO().BulkUpdateDocuments();
            MessageBox.Show("Finished!");
        }

        //
        private void button4_Click(object sender, EventArgs e)
        {
            new AWSElasticSearchServiceBO().DeleteIndexDefault();
            MessageBox.Show("Finished!");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var listDocuments = new AWSElasticSearchServiceBO().GetFullTextSearch(txtSearch.Text, txtClienteId.Text);
            dtIscas.DataSource = listDocuments.Documents;
        }

        private void txtClienteId_TextChanged(object sender, EventArgs e)
        {
            var listDocuments = new AWSElasticSearchServiceBO().GetFullTextSearch(txtSearch.Text, txtClienteId.Text);
            dtIscas.DataSource = listDocuments.Documents;
        }

        //
        private void button2_Click_1(object sender, EventArgs e)
        {
            var listDocuments = new AWSElasticSearchServiceBO().GetFullTextSearch(txtSearch.Text, txtClienteId.Text);
            dtIscas.DataSource = listDocuments.Documents;
        }
    }
}
