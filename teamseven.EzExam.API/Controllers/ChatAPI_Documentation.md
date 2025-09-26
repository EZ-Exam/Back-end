# Chat API Documentation

## Overview
This document explains how to use the Chat API endpoints for both ChatGPT and Gemini AI with automatic session management.

## Base URLs
- **ChatGPT**: `/api/ChatGPTChat`
- **Gemini**: `/api/GeminiChat`

## Authentication
All endpoints require JWT authentication. Include the Bearer token in the Authorization header:
```
Authorization: Bearer your-jwt-token
```

## Session Management

### How Sessions Work
- **Session ID**: Unique identifier for each conversation
- **Session Storage**: Stored in memory cache with automatic expiration
- **User Isolation**: Each user has separate sessions
- **Context Memory**: AI remembers conversation history within a session

### Session Lifecycle

#### 1. First Chat (New Session)
```json
POST /api/ChatGPTChat/chat
{
  "message": "Hello, my name is John"
}
```

**Response:**
```json
{
  "message": "Hello John! Nice to meet you.",
  "sessionId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "timestamp": "2025-09-26T01:42:24.8326578Z",
  "messageCount": 2
}
```

**Key Points:**
- Don't include `sessionId` in request
- System automatically creates new session
- Save the returned `sessionId` for future use

#### 2. Continue Conversation (Existing Session)
```json
POST /api/ChatGPTChat/chat
{
  "message": "What's my name?",
  "sessionId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

**Response:**
```json
{
  "message": "Your name is John, as you mentioned earlier.",
  "sessionId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "timestamp": "2025-09-26T01:43:15.1234567Z",
  "messageCount": 4
}
```

**Key Points:**
- Include `sessionId` from previous response
- AI remembers context from earlier messages
- Same `sessionId` is returned

#### 3. Start New Conversation
```json
POST /api/ChatGPTChat/chat
{
  "message": "Hello, I'm a new user"
}
```

**Response:**
```json
{
  "message": "Hello! Nice to meet you.",
  "sessionId": "b2c3d4e5-f6g7-8901-bcde-f23456789012",
  "timestamp": "2025-09-26T01:44:30.9876543Z",
  "messageCount": 2
}
```

**Key Points:**
- Don't include `sessionId` to start fresh
- New session ID is generated
- AI has no memory of previous conversations

## API Endpoints

### 1. Chat Endpoint
**POST** `/api/{ChatGPTChat|GeminiChat}/chat`

Send a message to AI with session management.

**Request Body:**
```json
{
  "message": "string (required)",
  "sessionId": "string (optional)"
}
```

**Response:**
```json
{
  "message": "string",
  "sessionId": "string",
  "timestamp": "datetime",
  "messageCount": "number"
}
```

### 2. Get History
**GET** `/api/{ChatGPTChat|GeminiChat}/history/{sessionId}`

Retrieve complete conversation history for a session.

**Response:**
```json
{
  "sessionId": "string",
  "messages": [
    {
      "role": "user|assistant",
      "content": "string",
      "timestamp": "datetime"
    }
  ],
  "messageCount": "number"
}
```

### 3. Clear History
**DELETE** `/api/{ChatGPTChat|GeminiChat}/history/{sessionId}`

Permanently delete conversation history for a session.

**Response:**
```json
{
  "message": "Conversation history cleared successfully"
}
```

### 4. Test Connection
**GET** `/api/{ChatGPTChat|GeminiChat}/test-connection`

Test API connection with a simple message.

**Response:**
```json
{
  "message": "API connection successful",
  "testResponse": "string",
  "timestamp": "datetime"
}
```

### 5. List Models (Gemini only)
**GET** `/api/GeminiChat/models`

List all available Gemini models.

**Response:**
```json
{
  "message": "Models retrieved successfully",
  "models": [
    {
      "name": "string",
      "displayName": "string",
      "description": "string",
      "supportedGenerationMethods": ["string"]
    }
  ],
  "timestamp": "datetime"
}
```

## Session Storage Details

### Cache Configuration
- **Storage**: In-memory cache
- **Expiration**: 30 minutes absolute, 10 minutes sliding
- **Size Limit**: 1000 entries with 25% compaction
- **User Isolation**: Each user's sessions are separate

### Session Key Format
```
{provider}_chat_session_{userId}_{sessionId}
```
Example: `chatgpt_chat_session_user123_a1b2c3d4-e5f6-7890-abcd-ef1234567890`

## Error Handling

### Common Error Responses

#### 401 Unauthorized
```json
{
  "message": "Invalid user token"
}
```

#### 400 Bad Request
```json
{
  "message": "Message cannot be empty"
}
```

#### 500 Internal Server Error
```json
{
  "message": "Internal server error"
}
```

## Frontend Integration Guide

### 1. First Chat Implementation
```javascript
// Start new conversation
const startChat = async (message) => {
  const response = await fetch('/api/ChatGPTChat/chat', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      message: message
      // Don't include sessionId for new chat
    })
  });
  
  const data = await response.json();
  // Save sessionId for future use
  localStorage.setItem('chatSessionId', data.sessionId);
  return data;
};
```

### 2. Continue Conversation
```javascript
// Continue existing conversation
const continueChat = async (message) => {
  const sessionId = localStorage.getItem('chatSessionId');
  
  const response = await fetch('/api/ChatGPTChat/chat', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      message: message,
      sessionId: sessionId // Include sessionId to maintain context
    })
  });
  
  return await response.json();
};
```

### 3. Start New Conversation
```javascript
// Start fresh conversation
const startNewChat = async (message) => {
  // Clear existing session
  localStorage.removeItem('chatSessionId');
  
  const response = await fetch('/api/ChatGPTChat/chat', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      message: message
      // Don't include sessionId to create new session
    })
  });
  
  const data = await response.json();
  localStorage.setItem('chatSessionId', data.sessionId);
  return data;
};
```

## Best Practices

### 1. Session Management
- Always save `sessionId` from first response
- Use `sessionId` for continuing conversations
- Don't include `sessionId` for new conversations
- Clear `sessionId` when starting fresh

### 2. Error Handling
- Check for 401 errors and redirect to login
- Handle 500 errors gracefully
- Implement retry logic for network issues

### 3. Performance
- Cache session IDs in localStorage
- Implement loading states
- Handle rate limiting (429 errors)

### 4. User Experience
- Show conversation history
- Provide "New Chat" button
- Display message timestamps
- Handle long responses

## Rate Limits

### ChatGPT
- **Free Tier**: 3 requests per minute
- **Paid Tier**: Higher limits based on plan

### Gemini
- **Free Tier**: 15 requests per minute
- **Paid Tier**: Higher limits based on plan

## Model Information

### ChatGPT Models
- **gpt-3.5-turbo**: Fast, cost-effective, good for most use cases
- **gpt-4**: More capable, higher cost
- **gpt-4-turbo**: Latest model with improved performance

### Gemini Models
- **gemini-1.0-pro**: Stable, good performance
- **gemini-1.5-flash**: Fast, cost-effective
- **gemini-1.5-pro**: More capable, higher cost

## Support

For technical support or questions about the Chat API, please contact the development team.
