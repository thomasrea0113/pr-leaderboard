import React, { useState, useEffect, useMemo } from 'react';
import 'react-dom';

import flow from 'lodash/fp/flow';
import groupBy from 'lodash/fp/groupBy';
import map from 'lodash/fp/map';
import User from '../serverTypes/User';
import UserView from '../serverTypes/UserView';
import Division from '../serverTypes/Division';

interface ReactProps {
    initialUrl: string;
    userName: string;
}

interface ReactState {
    user?: User;
    recommendations: UserView[];
    isLoading: boolean;
    hideBoards: { [key: string]: boolean };
}

const InitialState: ReactState = {
    recommendations: [],
    isLoading: true,
    hideBoards: {},
};

interface DivisionBoards {
    division: Division;
    boards: UserView[];
}

const groupByDivision = (boards: UserView[]): DivisionBoards[] =>
    flow(
        groupBy((b: UserView) => b.division.id),
        map(b => {
            return {
                division: b[0].division,
                boards: b,
            };
        })
    )(boards);

const RecommendationsComponent = (props: ReactProps) => {
    const [state, setState] = useState(InitialState);
    const { recommendations, isLoading, hideBoards } = state;

    const { initialUrl, userName } = props;

    // load initial data
    useEffect(() => {
        fetch(initialUrl)
            .then(response => response.json())
            .then(json =>
                setState({
                    ...state,
                    ...json,
                    isLoading: false,
                })
            );
    }, []);

    const data = useMemo(() => groupByDivision(recommendations), [
        recommendations,
    ]);

    const message = (loading: boolean): string => {
        if (loading)
            return `Hang tight, ${userName}! We're gathering some information for you.`;
        return "All done! Here's what we've got for you";
    };

    return (
        <div>
            <div>{message(isLoading)}</div>
            {data.map(d => (
                <div key={d.division.id} className="card mb-2">
                    <div className="card-header">{d.division.name}</div>
                    <img className="card-img-top" alt="Card cap" />
                    <div className="card-footer p-1">
                        <button
                            type="button"
                            onClick={() =>
                                setState({
                                    ...state,
                                    hideBoards: {
                                        ...hideBoards,
                                        [d.division.id]: !hideBoards[
                                            d.division.id
                                        ],
                                    },
                                })
                            }
                            className="btn btn-dark m-1 mb-2"
                        >
                            View Boards
                        </button>
                        <table
                            hidden={hideBoards[d.division.id]}
                            className="table table-sm table-dark mb-0"
                        >
                            <thead>
                                <tr>
                                    <th scope="col">Name</th>
                                    <th scope="col">Unit of Measure</th>
                                    <th scope="col">Weight Range</th>
                                </tr>
                            </thead>
                            <tbody>
                                {d.boards.map(b => (
                                    <tr key={b.id}>
                                        <th scope="row">{b.name}</th>
                                        <td>{b.uom.unit}</td>
                                        <td />
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default RecommendationsComponent;
