import React from 'react';

export const BoardComponent: React.FC<{
    name: string;
}> = ({ name }) => {
    return <div>board: {name}</div>;
};

export default BoardComponent;
