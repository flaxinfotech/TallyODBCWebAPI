using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TallyIntegrationAPI.Models;

namespace TallyIntegrationAPI.Services
{
    public class OdbcService
    {
        private readonly string _connectionString;

        public OdbcService(IOptions<TallyConfiguration> config)
        {
            // Set the connection string once using the configuration
            var dsn = config.Value.OdbcDsn;
            _connectionString = $"DSN={dsn}";
        }

        // Add or update a ledger
        public async Task<string> AddOrUpdateLedgerAsync(string ledgerName, string parentGroup, string address, string email, bool isUpdate)
        {
            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = isUpdate
                        ? "UPDATE Ledger SET $Address = ?, $Email = ? WHERE $Name = ?"
                        : "INSERT INTO Ledger ($Name, $Parent, $Address, $Email, $Country, $IsBillwiseOn) VALUES (?, ?, ?, ?, 'India', 'No')";

                    using (var command = new OdbcCommand(query, connection))
                    {
                        if (isUpdate)
                        {
                            command.Parameters.AddWithValue("Address", address);
                            command.Parameters.AddWithValue("Email", email);
                            command.Parameters.AddWithValue("LedgerName", ledgerName);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("LedgerName", ledgerName);
                            command.Parameters.AddWithValue("ParentGroup", parentGroup);
                            command.Parameters.AddWithValue("Address", address);
                            command.Parameters.AddWithValue("Email", email);
                        }

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0 ? "Ledger added/updated successfully." : "No rows affected.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ODBC Error: {ex.Message}");
            }
        }

        // Retrieve all ledgers
        public async Task<List<LedgerRequest>> GetAllLedgersAsync()
        {
            List<LedgerRequest> ledgers = new List<LedgerRequest>();

            try
            {
                using (OdbcConnection connection = new OdbcConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"SELECT $Name, $Parent, $Address[1], $Address[2], $CountryName, $StateName, $Pincode, $GSTRegistrationType, $GSTIN, $LedgerContact, $Email, $OpeningBalance FROM Ledger"; // No WHERE clause to fetch all ledgers

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        using (OdbcDataReader reader = (OdbcDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ledgers.Add(new LedgerRequest
                                {
                                    LedgerName = reader["$Name"]?.ToString(),
                                    ParentGroup = reader["$Parent"]?.ToString(),
                                    AddressLine1 = reader["$Address[1]"]?.ToString(),
                                    AddressLine2 = reader["$Address[2]"]?.ToString(),
                                    CountryName = reader["$CountryName"]?.ToString(),
                                    StateName = reader["$StateName"]?.ToString(),
                                    Pincode = reader["$Pincode"]?.ToString(),
                                    GSTRegistrationType = reader["$GSTRegistrationType"]?.ToString(),
                                    GSTIN = reader["$GSTIN"]?.ToString(),
                                    ContactPerson = reader["$LedgerContact"]?.ToString(),
                                    Email = reader["$Email"]?.ToString(),
                                    OpeningBalance = reader["$OpeningBalance"] != DBNull.Value
                                        ? Convert.ToDecimal(reader["$OpeningBalance"])
                                        : 0
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching ledgers: {ex.Message}", ex);
            }

            return ledgers;
        }

        public async Task<List<object>> GetFilteredLedgersAsync(string ledgerName = null, string parentGroup = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var ledgers = new List<object>();

            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = new StringBuilder("SELECT $Name, $Parent, $Date FROM Ledger WHERE 1=1");

                    if (!string.IsNullOrEmpty(ledgerName))
                        query.Append(" AND $Name = ?");
                    if (!string.IsNullOrEmpty(parentGroup))
                        query.Append(" AND $Parent = ?");
                    if (startDate.HasValue)
                        query.Append(" AND $Date >= ?");
                    if (endDate.HasValue)
                        query.Append(" AND $Date <= ?");

                    using (var command = new OdbcCommand(query.ToString(), connection))
                    {
                        if (!string.IsNullOrEmpty(ledgerName))
                            command.Parameters.AddWithValue("LedgerName", ledgerName);
                        if (!string.IsNullOrEmpty(parentGroup))
                            command.Parameters.AddWithValue("ParentGroup", parentGroup);
                        if (startDate.HasValue)
                            command.Parameters.AddWithValue("StartDate", startDate.Value);
                        if (endDate.HasValue)
                            command.Parameters.AddWithValue("EndDate", endDate.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ledgers.Add(new
                                {
                                    Name = reader["$Name"].ToString(),
                                    Parent = reader["$Parent"].ToString(),
                                    Date = reader["$Date"].ToString()
                                });
                            }
                        }
                    }
                }

                return ledgers;
            }
            catch (Exception ex)
            {
                throw new Exception($"ODBC Error: {ex.Message}");
            }
        }
    }
}
