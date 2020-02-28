import React, { useState, useEffect } from 'react';
import 'react-dom';

import User from '../serverTypes/User';
import UserView from '../serverTypes/UserView';
import DivisionTable from '../Components/DivisionTable';

interface ReactProps {
    initialUrl: string;
    userName: string;
}

class ReactState {
    // eslint-disable-next-line no-useless-constructor
    public constructor(
        public user: User = {
            userName: '',
            email: '',
            interests: [],
            leaderboards: [],
        },
        public recommendations: UserView[] = [],
        public isLoading: boolean = true
    ) {}
}

const RecommendationsComponent = (props: ReactProps) => {
    const [{ recommendations, isLoading }, setState] = useState(
        new ReactState()
    );

    const { initialUrl, userName } = props;

    // load initial data
    useEffect(() => {
        fetch(initialUrl)
            .then(response => response.json())
            .then(json => {
                const {
                    user: serverUser,
                    recommendations: serverRecommendations,
                } = json;
                setState(
                    new ReactState(
                        serverUser as User,
                        serverRecommendations as UserView[],
                        false
                    )
                );
            });
    }, []);

    const data = isLoading ? [] : recommendations;
    const message = (loading: boolean): string => {
        if (loading)
            return `Hang tight, ${userName}! We're gathering some information for you.`;
        return "All done! Here's what we've got for you";
    };

    return (
        <div>
            <div>{message(isLoading)}</div>
            <DivisionTable boards={data} />
        </div>
    );
};

export default RecommendationsComponent;
