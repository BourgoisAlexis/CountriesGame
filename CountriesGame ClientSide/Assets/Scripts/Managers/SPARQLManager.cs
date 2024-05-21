using System;
using System.Net.Http;
using System.Threading.Tasks;
using VDS.RDF.Query;
using System.Threading;
using System.Text;

public class SPARQLManager : Imanager {
    public void Setup(params object[] parameters) {
        throw new NotImplementedException();
    }

    public async void ProcessQuery(int index, string command) {
        Utils.Log(this, "ProcessQuery", command);

        SparqlResultSet results = await ProcessQuery(command);
        StringBuilder b = new StringBuilder();

        if (command.Contains("#MostRecentValueQuery") || command.Contains("#NestedPropertyValueQuery") || command.Contains("#PropertyValueQuery")) {
            foreach (SparqlResult r in results) {
                string[] parts = r.Value("value").ToString().Split('^');

                if (parts[2].Contains("#decimal"))
                    b.Append($"{System.Xml.XmlConvert.ToDecimal(parts[0])}");
                else if (parts[2].Contains("#date"))
                    b.Append($"{System.Xml.XmlConvert.ToDateTime(parts[0])}");
            }
        }
        else if (command.Contains("#PropertyLabelQuery")) {
            foreach (SparqlResult r in results) {
                b.Append($"{r.Value("label")}");
                b.Append(';');
                b.Append($"{r.Value("description")}");
            }
        }
        else if (command.Contains("#InitQuery")) {
            foreach (SparqlResult r in results) {
                b.Append($"{Utils.GetEntityID(r)}={Utils.GetLabel(r)}");
                b.Append(';');
            }
        }

        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageQueryResult, index, b.ToString());
    }

    private async Task<SparqlResultSet> ProcessQuery(string command) {
        SparqlResultSet results = null;
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "CountriesGame/0.0 (alexisbourgois0@gmail.com)");
        Uri uriEndPoint = new Uri("https://query.wikidata.org/sparql");

        SparqlQueryClient client = new SparqlQueryClient(httpClient, uriEndPoint);

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        try {
            results = await client.QueryWithResultSetAsync(command, cancellationToken);
        }
        catch (AggregateException ex) {
            foreach (Exception e in ex.InnerExceptions) {
                TaskCanceledException canceled = e as TaskCanceledException;

                if (canceled != null)
                    Utils.Log(this, "ProcessQuery", $"TaskCanceled : {canceled.Message}");
                else
                    Utils.Log(this, "ProcessQuery", $"Exception: {e.GetType().Name}");
            }
        }

        return results;
    }
}