# nl-agent-sk-ollama-with-function

An educational example demonstrating how to build a **local AI agent** in **C# (.NET 10)** using **Microsoft Semantic Kernel Agents** and **Ollama**, including **class-based function calling (tools)** for real-world data access.

This project shows how to run a **reactive Semantic Kernel ChatCompletionAgent** locally, allow the model to automatically invoke tools, and enforce strict behavioral rules through prompt instructions.

> This project uses the *agent abstraction* provided by Semantic Kernel, but does **not** implement autonomous planning or multi-step goal execution.

## Overview

**What this project demonstrates:**

- How to build a **local AI agent** using Semantic Kernel Agents
- How to run an LLM locally via **Ollama**
- How to expose **class-based tools** using `[KernelFunction]`
- How to enable **automatic function calling**
- How to enforce deterministic tool usage via agent instructions
- How to implement a clean console-based chat loop

This example focuses on clarity and correctness rather than abstraction-heavy or fully autonomous agent architectures.

## Agent Model Clarification (Important)

In Semantic Kernel terminology, an **agent** is:

> A structured wrapper around an LLM that includes identity, instructions, tools, and execution behavior.

This project implements a:

**Reactive, tool-augmented agent**

It is **not** an autonomous or planning-based agentic system. Specifically:

### What this agent does
- Responds to user input
- Selects and calls tools automatically
- Follows strict behavioral rules
- Operates in a single-step request → response loop

### What this agent does NOT do
- No long-term goals
- No planning or task decomposition
- No self-reflection or correction loops
- No autonomous execution without user input

This distinction is intentional and keeps the example simple and educational.

## Prerequisites

- **.NET 10.0 SDK or later**  
  https://dotnet.microsoft.com/

- **Ollama** installed and running locally  
  https://ollama.ai/

- Required Ollama model:
  ```bash
  ollama pull llama3.2:3b
  ```
  Ollama runs by default at: `http://localhost:11434`

## Installation

Clone the repository:

```bash
git clone https://github.com/your-username/sk-ollama-semantic-kernel-world-time-agent.git
cd sk-ollama-semantic-kernel-world-time-agent
```

Restore dependencies:

```bash
dotnet restore
```

## Project Structure

```
.
├── Program.cs             # Agent setup and chat loop
├── WorldTimePlugin.cs     # Class-based Semantic Kernel tool
├── sk-ollama-time-agent.csproj
└── README.md
```

## Key Concepts

### Semantic Kernel Agent

This project uses ChatCompletionAgent from Microsoft.SemanticKernel.Agents to define:

- Agent identity
- System instructions
- Kernel configuration
- Tool invocation behavior

```csharp
var agent = new ChatCompletionAgent
{
    Name = "TimeAssistant",
    Instructions = "...",
    Kernel = kernel,
    Arguments = new KernelArguments(
        new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        })
};
```

### Ollama Integration

The agent uses a local Ollama-hosted model:

```csharp
builder.AddOllamaChatCompletion(
    "llama3.2:3b",
    new Uri("http://localhost:11434"));
```

- No API keys
- No cloud dependencies
- Fully local execution

### Tool (Function) Implementation

#### WorldTimePlugin

The project exposes a class-based tool using `[KernelFunction]`.

```csharp
public class WorldTimePlugin
{
    [KernelFunction("GetCityTime")]
    [Description("Get the current time for a specific city")]
    public string GetCityTime(string city)
    {
        ...
    }
}
```

#### Supported Cities

The tool currently supports:

- Zurich
- Geneva
- London
- New York
- Tokyo
- Sydney

If a city is not supported, the tool returns a controlled response.

### Tool Registration

The plugin is registered directly on the kernel:

```csharp
builder.Plugins.AddFromObject(new WorldTimePlugin());
```

This makes the tool automatically discoverable by the agent.

### Agent Rules & Enforcement

The agent is instructed to always use the tool when a city is mentioned:

```
RULES:
1. ALWAYS use the 'GetCityTime' tool when a user mentions a city.
2. ALWAYS format the time in 24-hour HH:MM format.
3. If a city is not supported, respond exactly with: 'I don't know.'
4. Keep responses brief and professional.
```

Tool invocation is handled automatically via:

```csharp
FunctionChoiceBehavior.Auto()
```

## Running the Project

### Start Ollama

Ensure Ollama is running:

```bash
ollama serve
```

Verify Ollama is responding:

```bash
curl http://localhost:11434/api/tags
```

### Run the Agent

```bash
dotnet run
```

You should see:

```
--- World Time Agent (Class-Based) ---
Ask about the time in Zurich, London, Tokyo, New York, or Sydney.
```

### Example Interaction

```
You: What time is it in Tokyo?
Agent: It is 14:37 in Tokyo.

You: What time is it in Berlin?
Agent: I don't know.
```

## Key Technologies

- .NET 10.0 (C# Console Application)
- Microsoft Semantic Kernel 1.68
- Semantic Kernel Agents Framework
- Ollama (local LLM runtime)
- Llama 3.2 (3B) model
- Automatic function calling

## Why This Example Matters

This repository intentionally demonstrates:

- Fully local AI agents
- No cloud APIs
- Deterministic tool usage
- Clean separation between reasoning and execution
- Minimal but production-aligned patterns

It serves as a solid foundation for:

- Tool-augmented agents
- Local-first AI development
- Agent orchestration patterns
- Future expansion into planning or multi-agent systems

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please submit an issue or open a Pull Request if you find a bug or have an enhancement idea.
