from PyQt5.QtWidgets import QVBoxLayout, QLabel, QLineEdit, QComboBox, QHBoxLayout, QFrame, QPushButton
from PyQt5.QtGui import QKeySequence,QIcon
from Flicker import SeqType, SeqCondition, SequenceBlock


class SequenceBuilder(QFrame):
    def __init__(self, flicker):
        super().__init__()
        self.Flicker = flicker
        self.MainLayout = QVBoxLayout()
        self.setLayout(self.MainLayout)
        self.InitSeq(self.MainLayout, flicker.sequence)

        BottomLayout=QHBoxLayout()
        closeButton=QPushButton("Close")
        closeButton.clicked.connect(self.hide)
        BottomLayout.addStretch(0)
        BottomLayout.addWidget(closeButton)
        self.MainLayout.addLayout(BottomLayout)

    def InitSeq(self, parent, seq: SequenceBlock, rootSeq=None):
        seqWidget = QFrame()
        seqWidget.setFrameStyle(QFrame.StyledPanel)
        seqLayout = QVBoxLayout()
        # for subseq
        frame = QFrame()

        # type
        ly1 = QHBoxLayout()
        l1 = QLabel("Type")
        l1.setMinimumWidth(55)
        typebox = QComboBox()
        typebox.setMinimumWidth(65)
        for types in (SeqType):
            typebox.addItem(str(types.name))

        def assigntype(seq, text, frame):
            seq.seqType = SeqType[text]
            if text == "Block" or text == "Loop":
                frame.show()
                container_value.hide()
            else:
                frame.hide()
                container_value.show()

        typebox.setCurrentText(seq.seqType.name)
        typebox.currentTextChanged.connect(lambda text, seq=seq, f=frame: assigntype(seq, text, frame))

        ly1.addWidget(l1)
        ly1.addWidget(typebox)
        ly1.addStretch(0)
        seqLayout.addLayout(ly1)

        # Condition
        ly2 = QHBoxLayout()
        l2 = QLabel("Condition")
        l2.setMinimumWidth(55)
        conditionBox = QComboBox()
        conditionBox.setMinimumWidth(65)
        for cond in (SeqCondition):
            conditionBox.addItem(str(cond.name))

        def assigncond(seq, text):
            seq.Condition = SeqCondition[text]
            if SeqCondition[text] == SeqCondition.KeyPress:
                l3.setText("Key to proceed")
                keyButton.show()
                valueEdit.hide()
            else:
                l3.setText("Time in seconds")
                keyButton.hide()
                valueEdit.show()

        conditionBox.setCurrentText(seq.Condition.name)
        conditionBox.currentTextChanged.connect(lambda text, seq=seq: assigncond(seq, text))
        ly2.addWidget(l2)
        ly2.addWidget(conditionBox)
        ly2.addStretch(0)
        seqLayout.addLayout(ly2)

        # Value
        ly3 = QHBoxLayout()
        l3 = QLabel("Value")
        valueEdit = QLineEdit(str(seq.value))
        keyButton = QPushButton()

        def keyget(e,widget:QPushButton,seq):
            widget.releaseKeyboard()
            widget.setText("Key:" + e.text().upper())
            seq.value=float(e.key())
            widget.keyPressEvent = lambda e:False

        def assignValue(text, seq,widget:QPushButton=None):
            if seq.Condition == SeqCondition.KeyPress:
                widget.setText("Press a key...")
                widget.grabKeyboard()
                widget.keyPressEvent = lambda e,widget=widget,seq=seq: keyget(e,widget,seq)
            else:
                seq.value = text

        valueEdit.textChanged.connect(lambda text, seq=seq: assignValue(text, seq))
        keyButton.clicked.connect(lambda b,seq=seq,w=keyButton:assignValue(b,seq,keyButton))
        keyButton.setText("Key: "+QKeySequence(int(seq.value)).toString())
        ly3.addWidget(l3)
        ly3.addWidget(valueEdit)
        ly3.addWidget(keyButton)
        ly3.addStretch(0)
        container_value=QFrame()
        container_value.setFrameStyle(QFrame.StyledPanel)
        container_value.setLayout(ly3)
        seqLayout.addWidget(container_value)
        if seq.Condition==SeqCondition.KeyPress:
            l3.setText("Key to proceed")
            valueEdit.hide()
        else:
            l3.setText("Time in seconds")
            keyButton.hide()
        if seq.seqType == SeqType.Block or seq.seqType == SeqType.Loop:
            container_value.hide()



        #subsequence
        subseqLayout = QVBoxLayout()
        frame.setLayout(subseqLayout)
        frame.setLineWidth(5)
        #frame.setFrameStyle(QFrame.StyledPanel)

        l4 = QLabel("Subsequences:")
        addButton = QPushButton("+")
        separation_line=QFrame()
        separation_line.setFrameStyle(QFrame.HLine)

        ly4 = QHBoxLayout()
        ly4.addWidget(l4)
        ly4.addWidget(addButton)
        ly4.addStretch(0)
        subseqLayout.addLayout(ly4)
        #subseqLayout.addWidget(separation_line)



        def addNew(p, s, root):
            seq.AddSeq(s)
            self.InitSeq(p, s, root)

        addButton.clicked.connect(lambda b, p=subseqLayout, s=SequenceBlock(), root=seq: addNew(p, s, root))
        seqLayout.addWidget(frame)
        if seq.seqType == SeqType.Active or seq.seqType == SeqType.Inactive:
            frame.hide()

        for subseq in seq.contained_sequence:
            self.InitSeq(subseqLayout, subseq, seq)

        # Final thing
        finalLayout = QHBoxLayout()
        Indicator_Line=QFrame()
        Indicator_Line.setFrameStyle(QFrame.VLine|QFrame.Sunken)
        Indicator_Line.setLineWidth(3)
        finalLayout.addWidget(Indicator_Line)
        finalLayout.addLayout(seqLayout)
        ly5 = QVBoxLayout()
        removeButton = QPushButton("-")
        removeButton.setMaximumWidth(30)

        def remove(p:SequenceBlock, s:SequenceBlock):
            p.removeSeq(s)
            seqWidget.hide()
            self.adjustSize()

        if rootSeq:
            removeButton.clicked.connect(lambda b, s=seq, p=rootSeq: remove(p, s))

        ly5.addWidget(removeButton)
        ly5.addStretch(0)
        finalLayout.addLayout(ly5)
        seqWidget.setLayout(finalLayout)
        parent.addWidget(seqWidget)