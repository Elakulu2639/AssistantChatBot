using System.Text.RegularExpressions;

namespace ChatBot.Server.Services
{
    public class NlpService : INlpService
    {
        private readonly Dictionary<string, string[]> _intentPatterns;
        private readonly Dictionary<string, string[]> _entityPatterns;

        public NlpService()
        {
            // Initialize intent patterns
            _intentPatterns = new Dictionary<string, string[]>
            {
                {
                    "HR", new[]
                    {
                        @"hr",
                        @"policies",
                        @"leave",
                        @"conduct",
                        @"performance",
                        @"employee",
                        @"self\s+service",
                        @"profile",
                        @"payslip",
                        @"payroll",
                        @"salary",
                        @"contact",
                        @"help",
                        @"issue"
                    }
                },
                {
                    "INVENTORY", new[]
                    {
                        @"inventory",
                        @"stock",
                        @"items",
                        @"reorder",
                        @"report",
                        @"summary",
                        @"add",
                        @"low",
                        @"threshold",
                        @"transfer",
                        @"warehouse",
                        @"expiry",
                        @"perishable"
                    }
                },
                {
                    "FINANCE", new[]
                    {
                        @"finance",
                        @"expense",
                        @"submit",
                        @"report",
                        @"reimbursements",
                        @"history",
                        @"salary",
                        @"process",
                        @"pay",
                        @"monthly",
                        @"purchase",
                        @"order",
                        @"po",
                        @"raise",
                        @"tax",
                        @"download",
                        @"documents",
                        @"budget",
                        @"allocation"
                    }
                },
                {
                    "SUPPORT", new[]
                    {
                        @"support",
                        @"password",
                        @"reset",
                        @"forgot",
                        @"login",
                        @"contact",
                        @"technical",
                        @"it",
                        @"access",
                        @"erp",
                        @"off\s*site",
                        @"vpn",
                        @"down",
                        @"issue",
                        @"integrate",
                        @"api",
                        @"third\s*party",
                        @"tools",
                        @"user\s+manual",
                        @"help",
                        @"documentation"
                    }
                }
            };

            // Initialize entity patterns
            _entityPatterns = new Dictionary<string, string[]>
            {
                {
                    "CATEGORY", new[]
                    {
                        @"hr",
                        @"inventory",
                        @"finance",
                        @"support"
                    }
                },
                {
                    "ACTION", new[]
                    {
                        @"apply",
                        @"request",
                        @"update",
                        @"find",
                        @"contact",
                        @"reorder",
                        @"add",
                        @"transfer",
                        @"track",
                        @"submit",
                        @"raise",
                        @"download",
                        @"reset",
                        @"access",
                        @"integrate"
                    }
                },
                {
                    "OBJECT", new[]
                    {
                        @"policies",
                        @"leave",
                        @"profile",
                        @"payslip",
                        @"items",
                        @"stock",
                        @"report",
                        @"expense",
                        @"salary",
                        @"tax",
                        @"documents",
                        @"budget",
                        @"password",
                        @"erp",
                        @"vpn",
                        @"tools"
                    }
                }
            };
        }

        public async Task<IntentResult> AnalyzeIntentAsync(string userMessage)
        {
            var normalizedMessage = userMessage.ToLower();
            var bestIntent = "UNKNOWN";
            var highestConfidence = 0.0;

            foreach (var intent in _intentPatterns)
            {
                var confidence = await CalculateConfidenceAsync(normalizedMessage, intent.Key);
                if (confidence > highestConfidence)
                {
                    highestConfidence = confidence;
                    bestIntent = intent.Key;
                }
            }

            var entities = await ExtractEntitiesAsync(normalizedMessage);

            return new IntentResult
            {
                Intent = bestIntent,
                Confidence = highestConfidence,
                Entities = entities
            };
        }

        public async Task<List<string>> ExtractEntitiesAsync(string userMessage)
        {
            var entities = new List<string>();
            var normalizedMessage = userMessage.ToLower();

            foreach (var entityType in _entityPatterns)
            {
                foreach (var pattern in entityType.Value)
                {
                    var matches = Regex.Matches(normalizedMessage, pattern, RegexOptions.IgnoreCase);
                    foreach (Match match in matches)
                    {
                        entities.Add($"{entityType.Key}:{match.Value}");
                    }
                }
            }

            return entities;
        }

        public async Task<double> CalculateConfidenceAsync(string userMessage, string intent)
        {
            if (!_intentPatterns.ContainsKey(intent))
                return 0.0;

            var normalizedMessage = userMessage.ToLower().Trim();
            var patterns = _intentPatterns[intent];
            var totalMatches = 0.0;
            var totalPatterns = patterns.Length;

            // First check for exact matches from CSV
            foreach (var pattern in patterns)
            {
                var cleanPattern = pattern.Replace(@"\s+", " ").Replace(@"\s*", " ").Trim();
                if (normalizedMessage.Equals(cleanPattern, StringComparison.OrdinalIgnoreCase))
                {
                    return 1.0; // Perfect match
                }
            }

            // Then check for partial matches
            foreach (var pattern in patterns)
            {
                var cleanPattern = pattern.Replace(@"\s+", " ").Replace(@"\s*", " ").Trim();

                // Check if the pattern is a complete word or phrase in the message
                if (normalizedMessage.Contains(cleanPattern))
                {
                    totalMatches += 1.0;
                }
                else
                {
                    // Check for individual words in the pattern
                    var patternWords = cleanPattern.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var messageWords = normalizedMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var matchingWords = patternWords.Count(pw =>
                        messageWords.Any(mw => mw.Equals(pw, StringComparison.OrdinalIgnoreCase)));

                    if (matchingWords > 0)
                    {
                        totalMatches += matchingWords / (double)patternWords.Length;
                    }
                }
            }

            // Calculate confidence based on the ratio of matched patterns
            var baseConfidence = totalMatches / totalPatterns;

            // Boost confidence for good matches
            if (baseConfidence > 0.7)
            {
                return 0.95; // Very high confidence for very good matches
            }
            else if (baseConfidence > 0.5)
            {
                return 0.9; // High confidence for good matches
            }
            else if (baseConfidence > 0.3)
            {
                return 0.8; // Medium-high confidence for decent matches
            }
            else if (baseConfidence > 0)
            {
                return 0.7; // Medium confidence for minimal matches
            }

            return 0.0;
        }
    }
}