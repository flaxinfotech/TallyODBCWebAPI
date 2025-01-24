using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<string> AddOrUpdateLedgerAsync(string ledgerName, string parentGroup, string address, string email)
        {
            var xmlRequest = $@"
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
                                    <LEDGER NAME=""{ledgerName}"" RESERVEDNAME="""">
                                        <ADDRESS.LIST>
                                            <ADDRESS>{address}</ADDRESS>
                                        </ADDRESS.LIST>
                                        <LEDGERGROUPNAME>{parentGroup}</LEDGERGROUPNAME>
                                        <EMAIL>{email}</EMAIL>
                                        <PARENT>{parentGroup}</PARENT>
                                    </LEDGER>
                                </TALLYMESSAGE>
                            </REQUESTDATA>
                        </IMPORTDATA>
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
