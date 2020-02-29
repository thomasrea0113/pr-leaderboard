import React from 'react';

export const Board: React.FC<{
    name: string;
}> = ({ name }) => {
    return <div>board: {name}</div>;
};

export default Board;
