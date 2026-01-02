import React, { useRef, useEffect } from "react";

interface AsteroidsDisplayProps {
  ships: Array<Ship>;
  playerId?: number;
  asteroids: Array<Asteroid>;
  stars: Array<{ x: number; y: number;}>;
  bullets: Array<InternalGameObject>;
  leaderboard: Array<ScoreType>;
  pressedKeys: Set<string>;
  onPressedKeysChange: (keys: Set<string>) => void;
  gameSize: {width: number, height: number};
}

const ReactAsteroids: React.FC<AsteroidsDisplayProps> = ({
  ships,
  playerId = undefined,
  asteroids,
  stars,
  bullets = [],
  leaderboard = [],
  pressedKeys,
  onPressedKeysChange,
  gameSize
}) => {
  const divRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      event.preventDefault();
      const newKeys = new Set(pressedKeys);
      newKeys.add(event.key);
      onPressedKeysChange(newKeys);
    };

    const handleKeyUp = (event: KeyboardEvent) => {
      const newKeys = new Set(pressedKeys);
      newKeys.delete(event.key);
      onPressedKeysChange(newKeys);
    };

    const currentDiv = divRef.current;
    if (currentDiv) {
      currentDiv.addEventListener('keydown', handleKeyDown);
      currentDiv.addEventListener('keyup', handleKeyUp);
    }

    return () => {
      if (currentDiv) {
        currentDiv.removeEventListener('keydown', handleKeyDown);
        currentDiv.removeEventListener('keyup', handleKeyUp);
      }
    };
  }, [pressedKeys, onPressedKeysChange]);

  return (
    <>
      <div id='game' ref={divRef} tabIndex={0} style={{
        width: `${gameSize.width}px`,
        height: `${gameSize.height}px`,
        border: '2px solid rgba(255, 255, 255, 0.3)',
        margin: '0 auto',
        position: 'relative'
      }}>
        {ships.map((ship) => (
          <GameObject 
            class={ship.id === playerId ? "player" : "ship"} 
            object={{...ship, radius: 8}} 
            key={ship.id} 
          />
        ))}
        {asteroids.map((asteroid, i) => (
          <GameObject class={"asteroid"} object={asteroid} key={i + "asteroid"} />
        ))}
        {bullets.map((bullet, i) => (
          <GameObject class={"bullet"} object={bullet} key={i + "bullet"} />
        ))}
        {stars.map((star, i) => (
          <GameObject class={"star"} object={{...star, orientation: 0, radius: 0}} key={i + "star"} />
        ))}
{/* 
        <div
          className={`animated-element ${animationTriggered ? 'animate' : ''}`}
          style={{ top: position.y, left: position.x }}
        /> */}

        <Leaderboard scores={leaderboard} />
      </div>
    </>
  );
};

interface GameObjectProps {
  class: string;
  object: {
    x: number;
    y: number;
    orientation: number;
    radius?: number;
  };
}

const GameObject: React.FC<GameObjectProps> = ({ class: className, object }) => {
  let style: React.CSSProperties = {};
  if (!isNaN(object.x)) {
    style = {
      top: object.y,
      left: object.x,
      transform: `rotate(${object.orientation}deg)`
    };
    if (object.radius) {
      style.width = style.height = object.radius + "px";
    }
  }
  return <div className={className} style={style} />;
};

interface LeaderboardProps {
  scores: Array<ScoreType>;
}

const Leaderboard: React.FC<LeaderboardProps> = ({ scores }) => {
  return (
    <div id='leaderboard' style={{ right: 0 }}>
      <h2>Leaderboard</h2>
      {scores.map((entry, i) => (
        <p className='score' key={i}>{entry.id} ({entry.health}): {entry.score}</p>
      ))}
    </div>
  );
};

export default ReactAsteroids;