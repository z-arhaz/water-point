using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void calculate(object sender, EventArgs e)
    {
        Hashtable villageData = new Hashtable();
        Hashtable brokenPoints = new Hashtable();
        var client = new RestClient("https://raw.githubusercontent.com/onaio/ona-tech/master/data/water_points.json");
        var request = new RestRequest(Method.GET);
        IRestResponse response = client.Execute(request);
        Newtonsoft.Json.Linq.JArray obj1 = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
        int workingWaterPoints = 0;
        
        for (int i = 0; i < obj1.Count; i++)
        {
            JObject jobj_form = (JObject)obj1[i];
            int count = 1;
            int badcount = 1;
            string functioning_water_point = jobj_form.GetValue("water_functioning").ToString();
            if (functioning_water_point == "yes")
            {
                workingWaterPoints = workingWaterPoints + 1;
                string community = jobj_form.GetValue("communities_villages").ToString();
                if (villageData.Contains(community))
                {
                    string value = villageData[community].ToString();
                    int x = Int32.Parse(value);
                    x = x + 1;
                    villageData[community] = x;
                }
                else
                {
                    villageData.Add(community, count);
                }
            }
            else
            {
                string community = jobj_form.GetValue("communities_villages").ToString();
                if (brokenPoints.Contains(community))
                {
                    string value = brokenPoints[community].ToString();
                    int x = Int32.Parse(value);
                    x = x + 1;
                    brokenPoints[community] = x;
                }
                else
                {
                    brokenPoints.Add(community, badcount);
                }
            }
        }
        int totalBroken = brokenPoints.Count;
        int z=0;
        string percentiles = "";   
        string[][] Ranking = new string[totalBroken][];
        foreach (DictionaryEntry de in brokenPoints)
        {
            Ranking[z] = new string[2];
            Ranking[z][0] = de.Key.ToString();
            decimal broken = Convert.ToInt32(de.Value);
            decimal percentagebroken = 100* (broken / totalBroken);
            percentagebroken = Math.Round(percentagebroken, 2);
            Ranking[z][1] = percentagebroken.ToString();
            z++;
        }
        var dict = villageData.Cast<DictionaryEntry>().ToDictionary(d => d.Key, d => d.Value);
        string contextresponse = JsonConvert.SerializeObject(dict);
        string workingWP = JsonConvert.SerializeObject(workingWaterPoints);
        percentiles = JsonConvert.SerializeObject(Ranking);
        StringBuilder format = new StringBuilder();
        StringBuilder communityData = new StringBuilder();
        contextresponse = contextresponse.Replace("{", "{<p>");
        contextresponse = contextresponse.Replace(",", "</p><p>");
        contextresponse = contextresponse.Replace("}", "}</p>");
        format.Append("<p><b> Total Functioning WaterPoints : </b>" + workingWP + "</p>");
        HttpContext.Current.Response.Write(format);
        HttpContext.Current.Response.Write(contextresponse);
        HttpContext.Current.Response.Write(percentiles);
    }
}