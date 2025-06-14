using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBot.Server.Models;

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
                        @"issue",
                        @"skills",
                        @"review",
                        @"advance",
                        @"insurance",
                        @"benefits",
                        @"harassment"
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
                        @"allocation",
                        @"payment",
                        @"terms",
                        @"approval"
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
                        @"perishable",
                        @"movement",
                        @"categories",
                        @"audit"
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
                        @"documentation",
                        @"system",
                        @"errors",
                        @"backup",
                        @"procedures"
                    }
                },
                {
                    "SALES", new[]
                    {
                        @"sales",
                        @"order",
                        @"customer",
                        @"performance",
                        @"targets",
                        @"return",
                        @"reports",
                        @"leads",
                        @"meeting",
                        @"pipeline",
                        @"quota",
                        @"revenue",
                        @"conversion"
                    }
                },
                {
                    "MARKETING", new[]
                    {
                        @"marketing",
                        @"campaign",
                        @"promotions",
                        @"social\s+media",
                        @"analytics",
                        @"email",
                        @"content",
                        @"audience",
                        @"channels",
                        @"roi",
                        @"engagement"
                    }
                },
                {
                    "PROJECT", new[]
                    {
                        @"project",
                        @"management",
                        @"progress",
                        @"resources",
                        @"expenses",
                        @"reports",
                        @"risks",
                        @"timeline",
                        @"budget",
                        @"milestones",
                        @"deliverables"
                    }
                },
                {
                    "CUSTOMER", new[]
                    {
                        @"customer",
                        @"service",
                        @"complaints",
                        @"feedback",
                        @"ticket",
                        @"satisfaction",
                        @"accounts",
                        @"escalations",
                        @"support",
                        @"resolution"
                    }
                },
                {
                    "COMPLIANCE", new[]
                    {
                        @"compliance",
                        @"requirements",
                        @"regulations",
                        @"standards",
                        @"policies",
                        @"deadlines",
                        @"audit",
                        @"issues",
                        @"training",
                        @"documents"
                    }
                },
                {
                    "TRAINING", new[]
                    {
                        @"training",
                        @"courses",
                        @"enroll",
                        @"progress",
                        @"plan",
                        @"sessions",
                        @"evaluation",
                        @"resources",
                        @"certifications",
                        @"skills"
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
                        @"support",
                        @"sales",
                        @"marketing",
                        @"project",
                        @"customer",
                        @"compliance",
                        @"training"
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
                        @"integrate",
                        @"create",
                        @"manage",
                        @"conduct",
                        @"enroll",
                        @"evaluate"
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
                        @"tools",
                        @"campaign",
                        @"resources",
                        @"complaints",
                        @"training"
                    }
                },
                {
                    "TYPE", new[]
                    {
                        @"policy",
                        @"document",
                        @"report",
                        @"ticket",
                        @"campaign",
                        @"project",
                        @"course",
                        @"audit",
                        @"meeting",
                        @"order"
                    }
                },
                {
                    "STATUS", new[]
                    {
                        @"pending",
                        @"approved",
                        @"rejected",
                        @"completed",
                        @"in\s+progress",
                        @"escalated",
                        @"resolved"
                    }
                }
            };
        }

        public async Task<IntentResult> AnalyzeIntentAsync(string userMessage, List<ChatHistory> chatHistory)
        {
            var normalizedMessage = userMessage.ToLower().Trim();
            var bestIntent = "UNKNOWN";
            var highestConfidence = 0.0;
            var detectedEntities = await GetEntitiesFromMessage(userMessage);

            foreach (var intentEntry in _intentPatterns)
            {
                var intent = intentEntry.Key;
                var confidence = await CalculateConfidenceAsync(userMessage, intent, chatHistory);

                if (confidence > highestConfidence)
                {
                    highestConfidence = confidence;
                    bestIntent = intent;
                }
            }

            return new IntentResult
            {
                Intent = bestIntent,
                Confidence = highestConfidence,
                Entities = detectedEntities
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

        public async Task<double> CalculateConfidenceAsync(string userMessage, string intent, List<ChatHistory> chatHistory)
        {
            if (!_intentPatterns.ContainsKey(intent))
                return 0.0;

            var normalizedMessage = userMessage.ToLower().Trim();
            var patterns = _intentPatterns[intent];
            var scores = new List<double>();
            var messageWords = normalizedMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 1. Exact Match (Highest Priority)
            foreach (var pattern in patterns)
            {
                var cleanPattern = pattern.Replace(@"\s+", " ").Replace(@"\s*", " ").Trim();
                if (normalizedMessage.Equals(cleanPattern, StringComparison.OrdinalIgnoreCase))
                {
                    return 1.0; // Perfect match
                }
            }

            // 2. Pattern Matching
            foreach (var pattern in patterns)
            {
                var cleanPattern = pattern.Replace(@"\s+", " ").Replace(@"\s*", " ").Trim();
                var patternWords = cleanPattern.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // 2.1 Complete Pattern Match
                if (normalizedMessage.Contains(cleanPattern))
                {
                    scores.Add(0.9); // High confidence for complete pattern match
                    continue;
                }

                // 2.2 Word-by-Word Match
                var matchingWords = patternWords.Count(pw =>
                    messageWords.Any(mw => mw.Equals(pw, StringComparison.OrdinalIgnoreCase)));

                if (matchingWords > 0)
                {
                    var wordMatchRatio = matchingWords / (double)patternWords.Length;
                    scores.Add(wordMatchRatio * 0.8); // Weight for partial matches
                }
            }

            // 3. Keyword Matching
            var uniquePatterns = patterns.SelectMany(p => p.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                       .Distinct()
                                       .ToList();

            var matchingKeywords = messageWords.Count(mw =>
                uniquePatterns.Any(p => mw.Contains(p) || p.Contains(mw)));

            if (matchingKeywords > 0)
            {
                var keywordScore = (matchingKeywords / (double)Math.Max(messageWords.Length, uniquePatterns.Count)) * 0.7;
                scores.Add(keywordScore);
            }

            // 4. Entity-based Confidence Boost
            var detectedEntities = await GetEntitiesFromMessage(userMessage);

            if (detectedEntities.Any())
            {
                var entityMatchScore = 0.0;
                var intentKeywords = patterns.SelectMany(p => p.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Select(k => k.ToLower().Trim()).ToList();

                foreach (var entity in detectedEntities)
                {
                    var entityValue = entity.Split(':').Last().Trim(); // Get the actual entity value
                    // Check if the entity value is in the intent's keywords
                    if (intentKeywords.Any(ik => ik.Contains(entityValue) || entityValue.Contains(ik)))
                    {
                        entityMatchScore += 0.1; // Add a general boost for entity-keyword alignment
                    }
                }
                // Cap the entity match score to prevent it from dominating
                scores.Add(Math.Min(entityMatchScore, 0.3)); // Max boost of 0.3 from entities
            }

            // 5. Contextual Confidence Boost (New)
            if (chatHistory != null && chatHistory.Any())
            {
                var lastBotResponse = chatHistory.LastOrDefault()?.BotResponse.ToLower().Trim();
                var lastUserMessage = chatHistory.LastOrDefault()?.UserMessage.ToLower().Trim();

                // Boost if current intent matches last turn's intent or if keywords overlap
                foreach (var turn in chatHistory)
                {
                    var turnIntent = turn.Intent.ToLower();
                    if (turnIntent == intent.ToLower() && turnIntent != "unknown")
                    {
                        scores.Add(0.2); // Boost for consistent intent across turns
                        break;
                    }
                    // Add keyword overlap with previous messages for broader context
                    var previousWords = (turn.UserMessage + " " + turn.BotResponse).ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (messageWords.Any(mw => previousWords.Any(pw => pw.Equals(mw))))
                    {
                        scores.Add(0.1); // Small boost for general keyword overlap
                    }
                }
            }

            // 6. Calculate Final Score
            if (!scores.Any())
                return 0.0;

            var finalScore = scores.Max();

            // 7. Apply Confidence Thresholds
            if (finalScore > 0.8)
                return 0.95; // Very high confidence
            else if (finalScore > 0.6)
                return 0.85; // High confidence
            else if (finalScore > 0.4)
                return 0.75; // Medium confidence
            else if (finalScore > 0.2)
                return 0.65; // Low confidence

            return 0.0; // No confidence
        }

        private async Task<List<string>> GetEntitiesFromMessage(string userMessage)
        {
            var entities = new List<string>();
            var normalizedMessage = userMessage.ToLower().Trim();

            foreach (var entityType in _entityPatterns)
            {
                var type = entityType.Key;
                foreach (var pattern in entityType.Value)
                {
                    var cleanPattern = pattern.Replace(@"\s+", " ").Replace(@"\s*", " ").Trim();
                    if (normalizedMessage.Contains(cleanPattern))
                    {
                        entities.Add($"{type}:{cleanPattern}");
                    }
                }
            }
            return entities;
        }
    }
}