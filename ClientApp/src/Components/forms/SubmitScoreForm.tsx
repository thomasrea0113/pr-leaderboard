import React from 'react';
import { Unit } from '../../types/dotnet-types';
import { UnitIcon, Validator } from '../StyleComponents';
import { FieldProps } from './Validation';

export interface SubmitScoreProps {
    fieldAttributes?: FieldProps<SubmitScore>;
    unit: keyof typeof Unit;
}

export interface SubmitScore {
    userName: string;
    boardId: string;
    score: number;
}

export const SubmitScoreForm: React.FC<SubmitScoreProps> = ({
    unit,
    fieldAttributes,
}) => (
    <>
        <div className="form-group">
            <label htmlFor="score">Score</label>
            <div className="input-group">
                <div className="input-group-prepend">
                    <span className="input-group-text">
                        <UnitIcon forUnit={Unit[unit]} />
                    </span>
                </div>
                <input
                    {...fieldAttributes?.score.attributes}
                    className="form-control"
                />
                <div className="input-group-append">
                    <span className="input-group-text">{unit}</span>
                </div>
            </div>
            <Validator forProp={fieldAttributes?.score} />
            <Validator forProp={fieldAttributes?.boardId} />
            <Validator forProp={fieldAttributes?.userName} />
        </div>
        <input {...fieldAttributes?.boardId.attributes} type="hidden" />
        <input {...fieldAttributes?.userName.attributes} type="hidden" />
    </>
);
