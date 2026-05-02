const http = require('http');
const { Server } = require('socket.io');
const sql = require('mssql');

// 🛡️ SQL SERVER CONFIGURATION
// Replace these with your actual SQL Server credentials
const dbConfig = {
    user: 'sa',             // Your SQL Username
    password: 'YourPassword', // Your SQL Password
    server: 'localhost',    // Your Server URL/IP
    database: 'TestDB',     // Your Database Name (same as in appsettings.json)
    options: {
        encrypt: true, 
        trustServerCertificate: true 
    }
};

// Connect to SQL Server
async function connectDB() {
    try {
        await sql.connect(dbConfig);
        console.log('✅ Connected to SQL Server (TestDB)');
        
        // Ensure ChatMessages table exists
        await sql.query(`
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatMessages')
            BEGIN
                CREATE TABLE ChatMessages (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Sender NVARCHAR(255),
                    Receiver NVARCHAR(255),
                    MessageText NVARCHAR(MAX),
                    MessageTime NVARCHAR(100),
                    Timestamp DATETIME DEFAULT GETDATE()
                )
            END
        `);
        console.log('📊 ChatMessages table verified in SQL Server');
    } catch (err) {
        console.error('❌ SQL Server connection failed:', err.message);
        console.log('⚠️ Ensure SQL Server is running and TCP/IP is enabled in SQL Configuration Manager');
    }
}

connectDB();

const server = http.createServer();
const io = new Server(server, {
    cors: {
        origin: ["http://localhost:4200"],
        methods: ["GET", "POST"]
    }
});

io.on('connection', (socket) => {
    console.log('⚡ User connected:', socket.id);

    socket.on('join', (userId) => {
        socket.join(userId);
        console.log(`👤 User joined room: ${userId}`);
    });

    // FETCH HISTORY from SQL Server
    socket.on('get_history', async (data) => {
        const { me, other } = data;
        try {
            console.log(`🔎 Querying history: ${me} <-> ${other}`);
            const result = await sql.query`
                SELECT Sender as sender, Receiver as receiver, MessageText as text, MessageTime as time, Timestamp
                FROM ChatMessages 
                WHERE (Sender = ${me} AND Receiver = ${other}) 
                OR (Sender = ${other} AND Receiver = ${me}) 
                ORDER BY Timestamp ASC`;
            
            console.log(`📑 Found ${result.recordset.length} messages`);
            socket.emit('chat_history', result.recordset);
        } catch (err) {
            console.error('❌ History fetch error:', err.message);
        }
    });

    socket.on('send_message', async (data) => {
        const { to, from, text, time } = data;
        
        try {
            // 1. SAVE TO SQL SERVER
            await sql.query`
                INSERT INTO ChatMessages (Sender, Receiver, MessageText, MessageTime) 
                VALUES (${from}, ${to}, ${text}, ${time})`;
            
            console.log(`💾 Message saved to SQL Server from ${from} to ${to}`);

            // 2. DELIVER TO CONNECTED USER
            socket.to(to).emit('receive_message', { from, text, time });
        } catch (err) {
            console.error('❌ Error saving message to SQL Server:', err.message);
        }
    });

    socket.on('disconnect', () => {
        console.log('❌ User disconnected');
    });
});

const PORT = 3000;
server.listen(PORT, () => {
    console.log(`🚀 SQL Server Chat Hub running on http://localhost:${PORT}`);
});
