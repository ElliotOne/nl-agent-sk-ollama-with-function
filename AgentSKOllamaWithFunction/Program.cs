using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

// ----------------------
// 1. Setup Kernel & Agent
// ----------------------
var builder = Kernel.CreateBuilder();
builder.AddOllamaChatCompletion("llama3.2:3b", new Uri("http://localhost:11434"));

// Add the plugin to the kernel from the class object
builder.Plugins.AddFromObject(new WorldTimePlugin());

var kernel = builder.Build();

var agent = new ChatCompletionAgent
{
    Name = "TimeAssistant",
    Instructions = """
                   You are a precise World Time Assistant. 
                   Your primary goal is to provide the current time for requested cities.
                   
                   RULES:
                   1. ALWAYS use the 'GetCityTime' tool when a user mentions a city.
                   2. ALWAYS format the time in 24-hour HH:MM format (e.g., 14:30 instead of 2:30 PM).
                   3. If a city is not supported by the tool, your response must be exactly: 'I don't know.'
                   4. Keep responses brief and professional.
                   """,
    Kernel = kernel,
    Arguments = new KernelArguments(new PromptExecutionSettings
    {
        // This tells the agent to automatically call functions when it needs them
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    })
};

// ----------------------
// 2. Chat Loop
// ----------------------
var chatHistory = new ChatHistory();
Console.WriteLine("--- World Time Agent (Class-Based) ---");
Console.WriteLine("Ask about the time in Zurich, London, Tokyo, New York, or Sydney. (Type 'exit' to quit)");

while (true)
{
    Console.Write("\nYou: ");

    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage) || userMessage.ToLower() == "exit")
        break;

    chatHistory.AddUserMessage(userMessage);
    await foreach (var response in agent.InvokeAsync(new ChatMessageContent[]
    {
        new ChatMessageContent(AuthorRole.User, userMessage)
    }))
    {
        string agentText = response.Message.Content ?? "[No message returned]";
        chatHistory.AddAssistantMessage(agentText);
        Console.WriteLine($"Agent: {agentText}");
    }
}
