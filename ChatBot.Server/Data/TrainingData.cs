//using System.Collections.Generic;
//using ChatBot.Server.Models;

//namespace ChatBot.Server.Data
//{
//    public static class TrainingData
//    {
//        public static List<ChatTrainingData> GetTrainingData()
//        {
//            return new List<ChatTrainingData>
//            {
//                // Inventory Management
//                new ChatTrainingData
//                {
//                    Question = "How do I check stock levels?",
//                    Answer = "You can check stock levels through the Inventory Management module. Navigate to 'Stock > Current Stock' to view real-time inventory levels.",
//                    Category = "inventory",
//                    Keywords = new List<string> { "stock", "inventory", "levels", "check", "current" }
//                },
//                new ChatTrainingData
//                {
//                    Question = "How to create a purchase order?",
//                    Answer = "To create a purchase order, go to 'Purchase > New Order'. Fill in the supplier details, items, quantities, and prices. Click 'Submit' to create the order.",
//                    Category = "purchase",
//                    Keywords = new List<string> { "purchase", "order", "create", "new", "supplier" }
//                },

//                // HR Management
//                new ChatTrainingData
//                {
//                    Question = "How to process payroll?",
//                    Answer = "To process payroll, go to 'HR > Payroll > Process'. Select the pay period, review employee details, and click 'Process Payroll' to generate payments.",
//                    Category = "hr",
//                    Keywords = new List<string> { "payroll", "process", "salary", "payment", "employee" }
//                },
//                new ChatTrainingData
//                {
//                    Question = "How to manage employee leave?",
//                    Answer = "Employee leave can be managed in 'HR > Leave Management'. You can approve requests, view leave balances, and generate leave reports.",
//                    Category = "hr",
//                    Keywords = new List<string> { "leave", "employee", "vacation", "time off", "absence" }
//                },

//                // Finance
//                new ChatTrainingData
//                {
//                    Question = "How to generate financial reports?",
//                    Answer = "Financial reports can be generated from 'Finance > Reports'. Select the report type, date range, and click 'Generate' to create the report.",
//                    Category = "finance",
//                    Keywords = new List<string> { "financial", "reports", "generate", "statements", "accounts" }
//                },
//                new ChatTrainingData
//                {
//                    Question = "How to create an invoice?",
//                    Answer = "To create an invoice, go to 'Sales > Invoices > New'. Select the customer, add items, set prices, and click 'Generate Invoice'.",
//                    Category = "finance",
//                    Keywords = new List<string> { "invoice", "create", "bill", "customer", "payment" }
//                },

//                // Sales
//                new ChatTrainingData
//                {
//                    Question = "How to track sales performance?",
//                    Answer = "Sales performance can be tracked in 'Sales > Analytics'. View real-time metrics, sales trends, and performance reports by period.",
//                    Category = "sales",
//                    Keywords = new List<string> { "sales", "performance", "track", "analytics", "metrics" }
//                },
//                new ChatTrainingData
//                {
//                    Question = "How to manage customer information?",
//                    Answer = "Customer information can be managed in 'Sales > Customers'. Add new customers, update details, and view customer history.",
//                    Category = "sales",
//                    Keywords = new List<string> { "customer", "manage", "information", "details", "profile" }
//                },

//                // General
//                new ChatTrainingData
//                {
//                    Question = "What are the main features of the ERP system?",
//                    Answer = "Our ERP system includes modules for Inventory Management, HR Operations, Financial Management, Sales Operations, and Purchase Management. Each module is designed to streamline specific business processes.",
//                    Category = "general",
//                    Keywords = new List<string> { "features", "modules", "system", "erp", "capabilities" }
//                },
//                new ChatTrainingData
//                {
//                    Question = "How to get help with the system?",
//                    Answer = "You can get help through the 'Help' menu, contact IT support, or use this chatbot for quick assistance with common tasks.",
//                    Category = "general",
//                    Keywords = new List<string> { "help", "support", "assistance", "guide", "tutorial" }
//                }
//            };
//        }
//    }
//} 