import React from 'react';
import { NavLink, Route, useRouteMatch } from 'react-router-dom';
import { ApproveScoreComponent } from './ApproveScore';
import { NavProps } from '../../../pages/Board/NavProps';

export const ScoresNav: React.FC<NavProps> = ({ path }) => {
    return (
        <li className="nav-link dropdown">
            <NavLink
                className="nav-item dropdown-toggle"
                data-toggle="dropdown"
                to={path}
                role="button"
                aria-haspopup="true"
                aria-expanded="false"
            >
                Scores
            </NavLink>
            <div className="dropdown-menu">
                <NavLink className="dropdown-item" to={`${path}/Approve`}>
                    Approve Scores
                </NavLink>
            </div>
        </li>
    );
};

export const ScoresAdminComponent: React.FC<{}> = () => {
    // gets the relative path of the parent route
    const { path } = useRouteMatch();
    return (
        <>
            <h2>Scores Admin</h2>
            <Route path={`${path}/Approve`} component={ApproveScoreComponent} />
        </>
    );
};
