import React from 'react';
import { Unit } from '../../types/dotnet-types';
import { UnitIcon, ValidatorFor } from '../StyleComponents';
import { FieldProps, FieldPropInfo } from './Validation';

export interface SubmitScoreProps {
    fieldAttributes?: FieldProps<SubmitScore>;
    unit: keyof typeof Unit;
}

export interface SubmitScore {
    userName: string;
    boardId: string;
    score: number;
}

const validatorFor = (field?: FieldPropInfo) =>
    field != null ? <ValidatorFor forProp={field} /> : null;

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
            {validatorFor(fieldAttributes?.score)}
            {validatorFor(fieldAttributes?.boardId)}
            {validatorFor(fieldAttributes?.userName)}
        </div>
        <input {...fieldAttributes?.boardId.attributes} type="hidden" />
        <input {...fieldAttributes?.userName.attributes} type="hidden" />
    </>
);
