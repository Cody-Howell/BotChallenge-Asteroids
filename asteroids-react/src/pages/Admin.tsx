import { useState } from 'react'

function Admin() {
  const [adminKey, setAdminKey] = useState('');
  const [games, setGames] = useState<any[]>([]);
  const [newGameId, setNewGameId] = useState<number | null>(null);
  const [message, setMessage] = useState('');

  const fetchGames = async () => {
    try {
      const response = await fetch('/api/admin/games', {
        headers: {
          'Admin-Key': adminKey
        }
      });
      
      if (!response.ok) {
        setMessage(`Error: ${response.statusText}`);
        return;
      }
      
      const data = await response.json();
      setGames(data.games);
      setMessage(`Found ${data.count} games`);
    } catch (error) {
      setMessage(`Error fetching games: ${error}`);
    }
  };

  const createGame = async () => {
    try {
      const response = await fetch('/api/admin/game/create', {
        method: 'POST',
        headers: {
          'Admin-Key': adminKey
        }
      });
      
      if (!response.ok) {
        setMessage(`Error: ${response.statusText}`);
        return;
      }
      
      const data = await response.json();
      setNewGameId(data.gameId);
      setMessage(`Game created with ID: ${data.gameId}`);
      fetchGames();
    } catch (error) {
      setMessage(`Error creating game: ${error}`);
    }
  };

  const startGame = async (gameId: number) => {
    try {
      const response = await fetch(`/api/admin/game/start/${gameId}`, {
        method: 'POST',
        headers: {
          'Admin-Key': adminKey
        }
      });
      
      if (!response.ok) {
        setMessage(`Error: ${response.statusText}`);
        return;
      }
      
      const data = await response.json();
      setMessage(data.message);
      fetchGames();
    } catch (error) {
      setMessage(`Error starting game: ${error}`);
    }
  };

  const deleteGame = async (gameId: number) => {
    try {
      const response = await fetch(`/api/admin/game/${gameId}`, {
        method: 'DELETE',
        headers: {
          'Admin-Key': adminKey
        }
      });
      
      if (!response.ok) {
        setMessage(`Error: ${response.statusText}`);
        return;
      }
      
      const data = await response.json();
      setMessage(data.message);
      fetchGames();
    } catch (error) {
      setMessage(`Error deleting game: ${error}`);
    }
  };

  return (
    <div style={{ padding: '2rem', maxWidth: '1200px', margin: '0 auto' }}>
      <h1>Admin Panel</h1>
      
      <div style={{ marginBottom: '2rem' }}>
        <label>
          Admin Key:{' '}
          <input
            type="password"
            value={adminKey}
            onChange={(e) => setAdminKey(e.target.value)}
            style={{ marginLeft: '1rem', padding: '0.5rem', width: '300px' }}
          />
        </label>
      </div>

      <div style={{ marginBottom: '2rem', display: 'flex', gap: '1rem' }}>
        <button onClick={createGame} style={{ padding: '0.5rem 1rem' }}>
          Create New Game
        </button>
        <button onClick={fetchGames} style={{ padding: '0.5rem 1rem' }}>
          Refresh Games
        </button>
      </div>

      {message && (
        <div style={{ 
          padding: '1rem', 
          marginBottom: '1rem', 
          backgroundColor: 'hsla(0, 0%, 20%, 1.00)', 
          borderRadius: '4px' 
        }}>
          {message}
        </div>
      )}

      {newGameId && (
        <div style={{ 
          padding: '1rem', 
          marginBottom: '1rem', 
          backgroundColor: 'hsla(125, 39%, 20%, 1.00)', 
          borderRadius: '4px' 
        }}>
          <strong>New Game Created!</strong> 
          <br />
          <a href={`/${newGameId}`} target="_blank" rel="noopener noreferrer">
            Open Game {newGameId}
          </a>
        </div>
      )}

      <h2>Games</h2>
      {games.length === 0 ? (
        <p>No games found. Create one to get started!</p>
      ) : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ backgroundColor: 'hsla(0, 0%, 20%, 1.00)' }}>
              <th style={{ padding: '0.5rem', border: '1px solid #ddd' }}>Game ID</th>
              <th style={{ padding: '0.5rem', border: '1px solid #ddd' }}>Started</th>
              <th style={{ padding: '0.5rem', border: '1px solid #ddd' }}>Over</th>
              <th style={{ padding: '0.5rem', border: '1px solid #ddd' }}>Players</th>
              <th style={{ padding: '0.5rem', border: '1px solid #ddd' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {games.map((game) => (
              <tr key={game.gameId}>
                <td style={{ padding: '0.5rem', border: '1px solid #ddd' }}>
                  <a href={`/${game.gameId}`} target="_blank" rel="noopener noreferrer">
                    {game.gameId}
                  </a>
                </td>
                <td style={{ padding: '0.5rem', border: '1px solid #ddd' }}>
                  {game.isStarted ? '✅' : '❌'}
                </td>
                <td style={{ padding: '0.5rem', border: '1px solid #ddd' }}>
                  {game.isOver ? '✅' : '❌'}
                </td>
                <td style={{ padding: '0.5rem', border: '1px solid #ddd' }}>
                  {game.playerCount}
                </td>
                <td style={{ padding: '0.5rem', border: '1px solid #ddd' }}>
                  <button 
                    onClick={() => startGame(game.gameId)}
                    disabled={game.isStarted}
                    style={{ marginRight: '0.5rem', padding: '0.25rem 0.5rem' }}
                  >
                    Start
                  </button>
                  <button 
                    onClick={() => deleteGame(game.gameId)}
                    style={{ padding: '0.25rem 0.5rem' }}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}

export default Admin
