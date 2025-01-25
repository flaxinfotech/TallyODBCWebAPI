using System.Text;
using Microsoft.Extensions.Options;
using TallyIntegrationAPI.Models;

namespace TallyIntegrationAPI.Services
{
    public class XmlService
    {
        private readonly string _serverUrl;

        public XmlService(IOptions<TallyConfiguration> config)
        {
            // Set the Tally HTTP server URL once using the configuration
            _serverUrl = config.Value.HttpServerUrl;
        }

        public async Task<string> SendLedgerToTallyAsync(LedgerRequest ledgerRequest)
        {
            // Generate XML for a single ledger entry
            string xmlData = $@"
                    <ENVELOPE>
                        <HEADER>
                            <TALLYREQUEST>Import Data</TALLYREQUEST>
                        </HEADER>
                        <BODY>
                            <IMPORTDATA>
                                <REQUESTDESC>
                                    <REPORTNAME>All Masters</REPORTNAME>
                                </REQUESTDESC>
                                <REQUESTDATA>
                                    <TALLYMESSAGE xmlns:UDF=""TallyUDF"">
                                        <LEDGER NAME=""{ledgerRequest.LedgerName}"" ACTION=""Create"">
                                            <NAME>{ledgerRequest.LedgerName}</NAME>
                                            <PARENT>{ledgerRequest.ParentGroup}</PARENT>
                                            <ADDRESS.LIST>
                                                <ADDRESS>{ledgerRequest.AddressLine1}</ADDRESS>
                                                <ADDRESS>{ledgerRequest.AddressLine2}</ADDRESS>
                                            </ADDRESS.LIST>
                                            <COUNTRYNAME>{ledgerRequest.CountryName}</COUNTRYNAME>
                                            <STATENAME>{ledgerRequest.StateName}</STATENAME>
                                            <PINCODE>{ledgerRequest.Pincode}</PINCODE>
                                            <GSTREGISTRATIONTYPE>{ledgerRequest.GSTRegistrationType}</GSTREGISTRATIONTYPE>
                                            <GSTIN>{ledgerRequest.GSTIN}</GSTIN>
                                            <LEDGERCONTACT>{ledgerRequest.ContactPerson}</LEDGERCONTACT>
                                            <EMAIL>{ledgerRequest.Email}</EMAIL>
                                            <OPENINGBALANCE>{ledgerRequest.OpeningBalance}</OPENINGBALANCE>
                                        </LEDGER>
                                    </TALLYMESSAGE>
                                </REQUESTDATA>
                            </IMPORTDATA>
                        </BODY>
                    </ENVELOPE>";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(xmlData, Encoding.UTF8, "application/xml");
                    HttpResponseMessage response = await client.PostAsync(_serverUrl, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return $"Success: {responseContent}";
                    }
                    else
                    {
                        return $"Error: {response.StatusCode} - {responseContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        public async Task<string> GetLedgersXmlAsync()
        {
            var xmlRequest = @"
                        <ENVELOPE>
                            <HEADER>
                                <TALLYREQUEST>Export Data</TALLYREQUEST>
                            </HEADER>
                            <BODY>
                                <EXPORTDATA>
                                    <REQUESTDESC>
                                        <REPORTNAME>List of Ledgers</REPORTNAME>
                                    </REQUESTDESC>
                                </EXPORTDATA>
                            </BODY>
                        </ENVELOPE>";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");
                    var response = await httpClient.PostAsync(_serverUrl, content);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"XML Error: {ex.Message}");
            }
        }
    }
}
