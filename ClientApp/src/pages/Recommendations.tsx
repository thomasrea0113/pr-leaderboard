import React, { useState, useEffect } from 'react';
import 'react-dom';

import PropTypes from 'prop-types';

import User from '../serverTypes/User';
import UserView from '../serverTypes/UserView';
import DivisionTable from '../Components/DivisionTable';

interface ReactProps {
    initialUrl: string;
    userName: string;
}

class ReactState {
    public user: User;

    public constructor(
        user?: User,
        public recommendations: UserView[] = [],
        public isLoading: boolean = true
    ) {
        this.user = user ?? {
            userName: '',
            email: '',
            interests: [],
            leaderboards: [],
        };
        this.isLoading = isLoading;
        this.recommendations = recommendations;
    }
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
            .then(json => setState({ isLoading: false, ...json }));
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

RecommendationsComponent.propTypes = {
    initialUrl: PropTypes.string.isRequired,
};

export default RecommendationsComponent;
