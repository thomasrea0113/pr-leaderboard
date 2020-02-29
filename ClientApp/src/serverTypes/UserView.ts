import Leaderboard from './Leaderboard';
import UnitOfMeasure from './UnitOfMeasure';
import Division from './Division';
import WeightClass from './WeightClass';

export default class UserViewModel extends Leaderboard {
    public constructor(
        public isMember: boolean,
        public isRecommended: boolean,
        id: string,
        name: string,
        uom: UnitOfMeasure,
        division: Division,
        weightClass: WeightClass
    ) {
        super(id, name, uom, division, weightClass);
    }
}
