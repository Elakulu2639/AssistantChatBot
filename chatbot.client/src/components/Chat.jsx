import React, { useState, useEffect, useRef } from 'react';
import './Chat.css';

function Chat() {
    const [messages, setMessages] = useState([]);
    const [input, setInput] = useState('');
    const [isTyping, setIsTyping] = useState(false);
    const [darkMode, setDarkMode] = useState(false);
    const [sessionId, setSessionId] = useState(null);
    const chatBoxRef = useRef(null);

    // Scroll to bottom when messages change
    useEffect(() => {
        if (chatBoxRef.current) {
            chatBoxRef.current.scrollTop = chatBoxRef.current.scrollHeight;
        }
    }, [messages]);

    // Format timestamp HH:mm
    const formatTime = (date) => {
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    };

    const restartChat = () => {
        setMessages([]);
        setInput('');
        setIsTyping(false);
        setSessionId(null); // Clear the session ID when restarting chat
    };

    const sendMessage = async () => {
        if (!input.trim()) return;

        const userMsg = { from: 'user', text: input, timestamp: new Date() };
        setMessages((prev) => [...prev, userMsg]);
        setInput('');
        setIsTyping(true);

        try {
            const response = await fetch('https://localhost:7105/api/Chat', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({
                    userMessage: input,
                    sessionId: sessionId // Include sessionId in the request
                }),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
            }

            const data = await response.json();
            if (!data.success) {
                throw new Error(data.message || 'Failed to get response');
            }

            // Store the sessionId from the first response
            if (!sessionId && data.sessionId) {
                setSessionId(data.sessionId);
            }

            const botMsg = { from: 'bot', text: data.data, timestamp: new Date() };
            setMessages((prev) => [...prev, botMsg]);
        } catch (error) {
            console.error('Error:', error);
            const errorMsg = { from: 'bot', text: `An error occurred: ${error.message}`, timestamp: new Date() };
            setMessages((prev) => [...prev, errorMsg]);
        } finally {
            setIsTyping(false);
        }
    };

    return (
        <div className={`chat-container ${darkMode ? 'dark-mode' : ''}`}>
            <div className="chat-header">
                <h1>ERP Support</h1>
                <div className="header-buttons">
                    <button
                        className="restart-chat"
                        onClick={restartChat}
                        aria-label="Restart chat"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8" />
                            <path d="M3 3v5h5" />
                        </svg>
                        Restart
                    </button>
                    <button
                        className="dark-mode-toggle"
                        onClick={() => setDarkMode((prev) => !prev)}
                        aria-label="Toggle dark mode"
                    >
                        {darkMode ? (
                            <>
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <circle cx="12" cy="12" r="5" />
                                    <line x1="12" y1="1" x2="12" y2="3" />
                                    <line x1="12" y1="21" x2="12" y2="23" />
                                    <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
                                    <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
                                    <line x1="1" y1="12" x2="3" y2="12" />
                                    <line x1="21" y1="12" x2="23" y2="12" />
                                    <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
                                    <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
                                </svg>
                                Light Mode
                            </>
                        ) : (
                            <>
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
                                </svg>
                                Dark Mode
                            </>
                        )}
                    </button>
                </div>
            </div>

            <div className="chat-box" ref={chatBoxRef}>
                {messages.length === 0 && (
                    <div className="welcome-message">
                        <h2>Welcome to ERP Support</h2>
                        <p>I'm here to help you with any questions about ERP systems, processes, or guidelines. Feel free to ask anything!</p>
                    </div>
                )}
                {messages.map((msg, idx) => (
                    <div key={idx} className={`chat-message ${msg.from}`}>
                        {msg.from === 'bot' && (
                            <div className="message-avatar">
                                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <rect x="3" y="11" width="18" height="10" rx="2" />
                                    <circle cx="12" cy="5" r="2" />
                                    <path d="M12 7v4" />
                                    <line x1="8" y1="16" x2="8" y2="16" />
                                    <line x1="16" y1="16" x2="16" y2="16" />
                                </svg>
                            </div>
                        )}
                        <div className="message-content">
                            {msg.text}
                        </div>
                        <div className="message-timestamp">{formatTime(msg.timestamp)}</div>
                    </div>
                ))}
                {isTyping && (
                    <div className="typing-indicator">
                        <div className="message-avatar">
                            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <rect x="3" y="11" width="18" height="10" rx="2" />
                                <circle cx="12" cy="5" r="2" />
                                <path d="M12 7v4" />
                                <line x1="8" y1="16" x2="8" y2="16" />
                                <line x1="16" y1="16" x2="16" y2="16" />
                            </svg>
                        </div>
                        <span className="dots">
                            <span></span>
                            <span></span>
                            <span></span>
                        </span>
                    </div>
                )}
            </div>

            <div className="chat-input">
                <input
                    type="text"
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && input.trim() && sendMessage()}
                    placeholder="Type your message..."
                    aria-label="Chat input"
                />
                <button
                    onClick={sendMessage}
                    disabled={!input.trim()}
                    aria-label="Send message"
                    className={!input.trim() ? 'disabled' : ''}
                >
                    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                        <line x1="22" y1="2" x2="11" y2="13" />
                        <polygon points="22 2 15 22 11 13 2 9 22 2" />
                    </svg>
                    Send
                </button>
            </div>
        </div>
    );
}

export default Chat;
