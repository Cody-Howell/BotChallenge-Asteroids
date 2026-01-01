import React, { useRef, useEffect } from "react";

interface AsteroidsDisplayProps {
  ships: Array<Ship>;
  playerId?: number;
  asteroids: Array<{ x: number; y: number; orientation: number; value: number }>;
  stars: Array<{ x: number; y: number;}>;
  bullets: Array<InternalGameObject>;
  leaderboard: Array<{ name: string; score: number }>;
  pressedKeys: Set<string>;
  onPressedKeysChange: (keys: Set<string>) => void;
}

const ReactAsteroids: React.FC<AsteroidsDisplayProps> = ({
  ships,
  playerId = undefined,
  asteroids,
  stars,
  bullets = [],
  leaderboard = [],
  pressedKeys,
  onPressedKeysChange
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
      <div id='game' ref={divRef} tabIndex={0}>
        {ships.map((ship) => (
          <GameObject 
            class={ship.id === playerId ? "player" : "ship"} 
            object={ship} 
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
          <GameObject class={"star"} object={{...star, orientation: 0, value: 0}} key={i + "star"} />
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
    value?: number;
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
    if (object.value && object.value > 0) {
      style.width = style.height = (object.value * 15) + "px";
    }
  }
  return <div className={className} style={style} />;
};

interface LeaderboardProps {
  scores: Array<{ name: string; score: number }>;
}

const Leaderboard: React.FC<LeaderboardProps> = ({ scores }) => {
  return (
    <div id='leaderboard' style={{ right: 0 }}>
      <h2>Leaderboard</h2>
      {scores.map((entry, i) => (
        <p className='score' key={i}>{entry.name}: {entry.score}</p>
      ))}
    </div>
  );
};

export default ReactAsteroids;