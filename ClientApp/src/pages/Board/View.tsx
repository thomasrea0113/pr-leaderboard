import React from 'react';
import 'react-dom'; // only needed if this is to be placed directly on the page

interface Props {
    initialUrl: string;
}

interface State {
    isLoading: boolean;
}

const ViewBoardComponent: React.FC<Props> = ({ initialUrl }) => {
    return <div>hello world {initialUrl}!</div>;
};

export default ViewBoardComponent;
