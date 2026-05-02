const http = require('http');
const { Server } = require('socket.io');
const sqlite3 = require('sqlite3').verbose();
const path = require('path');

// 🛡️ SQLITE CONFIGURATION (Best for free hosting like Render)
const dbPath = path.resolve(__dirname, 'chat.db');
const db = new sqlite3.Database(dbPath, (err) => {
    if (err) console.error('❌ SQLite connection failed:', err.message);
    else console.log('✅ Connected to SQLite (chat.db)');
});

// Ensure ChatMessages table exists
db.serialize(() => {
    db.run(`
        CREATE TABLE IF NOT EXISTS ChatMessages (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Sender TEXT,
            Receiver TEXT,
            MessageText TEXT,
            MessageTime TEXT,
            Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
        )
    `);
    console.log('📊 ChatMessages table verified');
});

const server = http.createServer();
const io = new Server(server, {
    cors: {
        origin: "*", 
        methods: ["GET", "POST"]
    }
});

io.on('connection', (socket) => {
    console.log('⚡ User connected:', socket.id);

    socket.on('join', (userId) => {
        socket.join(userId);
        console.log(`👤 User joined room: ${userId}`);
    });

    // FETCH HISTORY
    socket.on('get_history', (data) => {
        const { me, other } = data;
        const query = `
            SELECT Sender as sender, Receiver as receiver, MessageText as text, MessageTime as time, Timestamp
            FROM ChatMessages 
            WHERE (Sender = ? AND Receiver = ?) 
            OR (Sender = ? AND Receiver = ?) 
            ORDER BY Timestamp ASC`;
        
        db.all(query, [me, other, other, me], (err, rows) => {
            if (err) console.error('❌ History fetch error:', err.message);
            else socket.emit('chat_history', rows);
        });
    });

    socket.on('send_message', (data) => {
        const { to, from, text, time } = data;
        const query = `INSERT INTO ChatMessages (Sender, Receiver, MessageText, MessageTime) VALUES (?, ?, ?, ?)`;
        
        db.run(query, [from, to, text, time], function(err) {
            if (err) console.error('❌ Error saving message:', err.message);
            else {
                // Emit to the recipient
                socket.to(to).emit('receive_message', { from, text, time });
            }
        });
    });

    socket.on('disconnect', () => {
        console.log('❌ User disconnected');
    });
});

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
    console.log(`🚀 Chat Hub running on port ${PORT}`);
});
