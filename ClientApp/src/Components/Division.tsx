import React from 'react';
import { Row } from 'react-table';
import UserView from '../serverTypes/UserView';
import { Expander } from './StyleComponents';
import Board from './Board';

const DivisionComponent: React.FC<{
    row: Row<UserView>;
}> = ({ row }) => {
    return (
        <div className="card" {...row.getRowProps()}>
            <img className="card-img-top" src="..." alt="Card ap" />
            <div className="card-body">
                <div className="card-title">
                    {row.cells.map(cell => (
                        <span
                            key={row.id + cell.column.id}
                            className="col-sm-4"
                        >
                            {cell.column.Header} - {cell.render('Cell')}
                        </span>
                    ))}
                </div>
                <p className="card-text">
                    <Expander
                        props={row.getToggleRowExpandedProps()}
                        isExpanded={row.isExpanded}
                    />{' '}
                    View Boards
                    {row.isExpanded ? <Board name="hello world" /> : null}
                </p>
                <a href="?" className="btn btn-primary">
                    Go somewhere
                </a>
            </div>
        </div>
    );
};

export default DivisionComponent;
