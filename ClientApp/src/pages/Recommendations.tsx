import React, { Component } from 'react';
import PropTypes from 'prop-types';

interface User {
    userName: string;
    email: string;
}

interface RecommendationsState {
    user: User;
}

interface RecommendationsProps {
    initialUrl: string;
}

// eslint-disable-next-line react/prefer-stateless-function
export default class RecommendationsComponent extends Component<
    RecommendationsProps,
    RecommendationsState
> {
    public static get displayName(): string {
        return 'Recommendations';
    }

    public static get propTypes() {
        return {
            initialUrl: PropTypes.string.isRequired,
        };
    }

    public async componentDidMount() {
        const { initialUrl } = this.props;
        const response: RecommendationsState = await (
            await fetch(initialUrl)
        ).json();
        this.setState(response);
    }

    public render(): JSX.Element {
        if (this.state != null) {
            const {
                user: { userName },
            } = this.state;
            return <div>Here are your recommendations, {userName}</div>;
        }
        return <div>loading...</div>;
    }
}
